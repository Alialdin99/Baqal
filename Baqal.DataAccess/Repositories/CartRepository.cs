using Baqal.DataAccess.Interfaces;
using Baqal.DataContext;
using Baqal.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Baqal.DataAccess.Repositories
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartBySessionIdAsync(string sessionId)
        {
            return await _context.Carts
                                 .Include(c => c.Items)
                                 .FirstOrDefaultAsync(c => c.SessionId == sessionId);
        }

        // Helper method to get cart by EITHER userId or sessionId
        private async Task<Cart?> GetCartAsync(string? userId, string? sessionId)
        {
            if (!string.IsNullOrEmpty(userId))
                return await GetCartByUserIdAsync(userId);

            if (!string.IsNullOrEmpty(sessionId))
                return await GetCartBySessionIdAsync(sessionId);

            return null;
        }

        public async Task<Cart> AddItemAsync(string? userId, string? sessionId, Guid productId, int quantity)
        {
            var cart = await GetCartAsync(userId, sessionId);

            // Create new cart if doesn't exist
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    SessionId = sessionId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Carts.AddAsync(cart);
            }

            // Check if item already exists
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                // Update quantity if item exists
                existingItem.Quantity += quantity;
            }
            else
            {
                // Add new item
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task RemoveItemAsync(string? userId, string? sessionId, Guid productId)
        {
            var cart = await GetCartAsync(userId, sessionId);
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(string? userId, string? sessionId, Guid productId, int quantity)
        {
            var cart = await GetCartAsync(userId, sessionId);
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    // Remove item if quantity is 0 or negative
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string? userId, string? sessionId)
        {
            var cart = await GetCartAsync(userId, sessionId);
            if (cart == null) return;

            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // Merge guest cart into user cart when logging in
        public async Task MergeCartsAsync(string sessionId, string userId)
        {
            var guestCart = await GetCartBySessionIdAsync(sessionId);
            var userCart = await GetCartByUserIdAsync(userId);

            if (guestCart == null) return; // No guest cart to merge

            if (userCart == null)
            {
                // User has no cart, convert guest cart to user cart
                guestCart.UserId = userId;
                guestCart.SessionId = null;
                guestCart.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Merge items from guest cart to user cart
                foreach (var guestItem in guestCart.Items)
                {
                    var existingItem = userCart.Items
                        .FirstOrDefault(i => i.ProductId == guestItem.ProductId);

                    if (existingItem != null)
                    {
                        // Add quantities together
                        existingItem.Quantity += guestItem.Quantity;
                    }
                    else
                    {
                        // Add new item to user cart
                        userCart.Items.Add(new CartItem
                        {
                            ProductId = guestItem.ProductId,
                            Quantity = guestItem.Quantity,
                            AddedAt = guestItem.AddedAt
                        });
                    }
                }

                userCart.UpdatedAt = DateTime.UtcNow;

                // Delete guest cart after merge
                _context.Carts.Remove(guestCart);
            }

            await _context.SaveChangesAsync();
        }

        // Clean up old guest carts (run as scheduled job)
        public async Task DeleteOldGuestCartsAsync(int daysOld = 7)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);

            var oldCarts = await _context.Carts
                .Where(c => c.SessionId != null &&
                           c.UserId == null &&
                           c.UpdatedAt < cutoffDate)
                .ToListAsync();

            _context.Carts.RemoveRange(oldCarts);
            await _context.SaveChangesAsync();
        }
    }
}