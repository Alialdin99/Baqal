using Baqal.Entities.Models;

namespace Baqal.DataAccess.Interfaces
{
    public interface ICartRepository
    {
        // Getters
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart?> GetCartBySessionIdAsync(string sessionId);

        // Item operations support BOTH userId and sessionId
        Task<Cart> AddItemAsync(string? userId, string? sessionId, Guid productId, int quantity);
        Task RemoveItemAsync(string? userId, string? sessionId, Guid productId);
        Task UpdateQuantityAsync(string? userId, string? sessionId, Guid productId, int quantity);
        Task ClearCartAsync(string? userId, string? sessionId);

        // Merge guest cart into user cart when user logs in
        Task MergeCartsAsync(string sessionId, string userId);

        // Cleanup old guest carts (run as background job)
        Task DeleteOldGuestCartsAsync(int daysOld = 7);
    }
}