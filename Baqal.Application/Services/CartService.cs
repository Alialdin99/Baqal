using Baqal.DataAccess.Interfaces;
using Baqal.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Baqal.Application.Services
{
    public class CartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public CartService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        // ---------- Database-backed Cart (for authenticated users) ----------
        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
        }

        public async Task AddItemToUserCartAsync(string userId, string productId, int quantity)
        {
            await _unitOfWork.Carts.AddItemAsync(userId, productId, quantity);
            await _unitOfWork.Save();
        }

        public async Task RemoveItemFromUserCartAsync(string userId, string productId)
        {
            await _unitOfWork.Carts.RemoveItemAsync(userId, productId);
            await _unitOfWork.Save();
        }

        public async Task UpdateUserCartQuantityAsync(string userId, string productId, int quantity)
        {
            await _unitOfWork.Carts.UpdateQuantityAsync(userId, productId, quantity);
            await _unitOfWork.Save();
        }

        public async Task ClearUserCartAsync(string userId)
        {
            await _unitOfWork.Carts.ClearCartAsync(userId);
            await _unitOfWork.Save();
        }

        // ---------- Session-backed Cart (for guests) ----------
        private const string SessionCartKey = "SessionCart";

        public Task<List<CartItem>> GetCartBySessionIdAsync(string sessionId)
        {
            var cartJson = Session.GetString(SessionCartKey);
            if (string.IsNullOrEmpty(cartJson))
                return Task.FromResult(new List<CartItem>());

            var cart = JsonSerializer.Deserialize<List<CartItem>>(cartJson)!;
            return Task.FromResult(cart);
        }

        public Task AddItemAsync(string sessionId, string productId, int quantity)
        {
            var cartJson = Session.GetString(SessionCartKey);
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson)!;

            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.Add(new CartItem { ProductId = productId, Quantity = quantity });

            Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart));
            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(string sessionId, string productId)
        {
            var cart = GetCartFromSession();
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCartToSession(cart);
            return Task.CompletedTask;
        }

        public Task UpdateQuantityAsync(string sessionId, string productId, int quantity)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
                item.Quantity = quantity;

            SaveCartToSession(cart);
            return Task.CompletedTask;
        }

        public Task ClearCartAsync(string sessionId)
        {
            Session.Remove(SessionCartKey);
            return Task.CompletedTask;
        }

        // ---------- Helpers ----------
        private List<CartItem> GetCartFromSession()
        {
            var cartJson = Session.GetString(SessionCartKey);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson)!;
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            Session.SetString(SessionCartKey, JsonSerializer.Serialize(cart));
        }
    }
}
