using Baqal.Application.DTOs;
using Baqal.Application.DTOs.CartDTOs;
using Baqal.DataAccess.Interfaces;
using Baqal.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Baqal.Application.Services
{
    public class CartService 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Session key constant
        private const string SessionCartKey = "CartSessionId";

        public CartService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        private string GetOrCreateSessionId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session == null)
                throw new InvalidOperationException("Session is not available");

            var sessionId = session.GetString(SessionCartKey);

            if (string.IsNullOrEmpty(sessionId))
            {
                // Generate unique session ID
                sessionId = Guid.NewGuid().ToString();
                session.SetString(SessionCartKey, sessionId);
            }

            return sessionId;
        }

        /// Gets current authenticated user ID, or null if guest
        private string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true
                ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        /// Maps Cart entity to CartResponseDto with product details
        private async Task<CartResponseDto> MapToCartResponseDto(Cart cart)
        {
            var items = new List<CartItemResponseDto>();

            foreach (var item in cart.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    items.Add(new CartItemResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        Price = (decimal)product.Price,
                        Quantity = item.Quantity
                        // Subtotal calculated automatically in DTO
                    });
                }
            }

            return new CartResponseDto
            {
                CartId = cart.Id,
                Items = items
                // TotalItems calculated automatically in DTO
            };
        }


        /// Gets current user's cart (guest or authenticated)
        public async Task<CartResponseDto> GetCurrentCartAsync()
        {
            var userId = GetCurrentUserId();
            var sessionId = userId == null ? GetOrCreateSessionId() : null;

            Cart? cart;
            if (!string.IsNullOrEmpty(userId))
            {
                cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            }
            else if (!string.IsNullOrEmpty(sessionId))
            {
                cart = await _unitOfWork.Carts.GetCartBySessionIdAsync(sessionId);
            }
            else
            {
                return new CartResponseDto();
            }

            if (cart == null)
                return new CartResponseDto();

            return await MapToCartResponseDto(cart);
        }

        /// Adds item to cart (creates cart if doesn't exist)
        public async Task<CartResponseDto> AddItemAsync(CartItemDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(request.Quantity));

            //  Validate product exists using UnitOfWork
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with ID '{request.ProductId}' not found");

            // Check stock availability
            if (product.StockQuantity < request.Quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}, Requested: {request.Quantity}");

            var userId = GetCurrentUserId();
            var sessionId = userId == null ? GetOrCreateSessionId() : null;

            // Add item through UnitOfWork
            var cart = await _unitOfWork.Carts.AddItemAsync(
                userId,
                sessionId,
                request.ProductId,
                request.Quantity
            );


            return await MapToCartResponseDto(cart);
        }

        /// Updates quantity of existing cart item
        public async Task<CartResponseDto> UpdateItemAsync(CartItemDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(request.Quantity));

            // Validate product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with ID '{request.ProductId}' not found");

            // Optional: Check stock if increasing quantity
            if (request.Quantity > 0 && product.StockQuantity < request.Quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.StockQuantity}");

            var userId = GetCurrentUserId();
            var sessionId = userId == null ? GetOrCreateSessionId() : null;

            // Update through UnitOfWork
            await _unitOfWork.Carts.UpdateQuantityAsync(
                userId,
                sessionId,
                request.ProductId,
                request.Quantity
            );


            return await GetCurrentCartAsync();
        }

        /// Removes item from cart
        public async Task<CartResponseDto> RemoveItemAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty", nameof(productId));

            var userId = GetCurrentUserId();
            var sessionId = userId == null ? GetOrCreateSessionId() : null;

            // Remove through UnitOfWork
            await _unitOfWork.Carts.RemoveItemAsync(userId, sessionId, productId);

            return await GetCurrentCartAsync();
        }

        /// Clears all items from cart
        public async Task ClearCartAsync()
        {
            var userId = GetCurrentUserId();
            var sessionId = userId == null ? GetOrCreateSessionId() : null;

            await _unitOfWork.Carts.ClearCartAsync(userId, sessionId);
        }

        
        /// Merges guest cart into user cart after login
        /// Should be called immediately after successful authentication
        public async Task MergeGuestCartOnLoginAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            var session = _httpContextAccessor.HttpContext?.Session;
            var sessionId = session?.GetString(SessionCartKey);

            if (!string.IsNullOrEmpty(sessionId))
            {
                // Merge through UnitOfWork
                await _unitOfWork.Carts.MergeCartsAsync(sessionId, userId);

                // Single Save call
                // await _unitOfWork.Save(); // Already saved in repository

                // Clear session after merge
                session?.Remove(SessionCartKey);
            }
        }

        /// Gets cart item count for display in header
        public async Task<int> GetCartItemCountAsync()
        {
            var cart = await GetCurrentCartAsync();
            return cart.TotalItems;
        }

        /// Gets cart total price
        public async Task<decimal> GetCartTotalAsync()
        {
            var cart = await GetCurrentCartAsync();
            return cart.Items.Sum(i => i.Subtotal);
        }

        
    }
}