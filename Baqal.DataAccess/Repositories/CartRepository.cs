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

        public async Task AddItemAsync(string userId, string productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.Carts.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
                existingItem.Quantity += quantity;
            else
                cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(string userId, string productId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(string userId, string productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartByUserIdAsync(userId);
            if (cart == null) return;

            cart.Items.Clear();
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
