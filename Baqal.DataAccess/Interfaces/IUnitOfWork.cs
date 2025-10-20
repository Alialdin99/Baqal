
namespace Baqal.DataAccess.Interfaces 
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        public IStoreRepository Stores { get; }
        public IProductRepository Products { get; }

        public ICartRepository Carts {  get; }

        Task<int> SaveAsync();
        int Save();

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackAsync();    }
}
