using Baqal.DataAccess.Interfaces;
using Baqal.DataAccess.Repositories;
using Baqal.DataContext;

namespace Baqal.DataAccess
{
    public class UnitOfWork
    {
        private readonly AppDbContext _context;

        public IStoreRepository Stores { get; private set; }
        public IProductRepository Products { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Stores = new StoreRepository(_context);
            Products = new ProductRepository(_context);
            
        }

        public async void Dispose(){
            await _context.DisposeAsync();
        }

        public async Task<int> Save()
        { 
            return await _context.SaveChangesAsync();
        }

    }
}
