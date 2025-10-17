using Baqal.Entities.Models;

namespace Baqal.DataAccess.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart?> GetCartBySessionIdAsync(string sessionId);

        Task AddItemAsync(string userId, string productId, int quantity);
        Task RemoveItemAsync(string userId, string productId);
        Task UpdateQuantityAsync(string userId, string productId, int quantity);

        Task ClearCartAsync(string userId);
        Task SaveChangesAsync();
    }
}
