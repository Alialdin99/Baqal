using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Baqal.DataAccess.Interfaces;
using Baqal.DataAccess.Repositories;
using Baqal.DataContext;
using System.Runtime.CompilerServices;

namespace Baqal.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private IStoreRepository? _stores;
        private IProductRepository _products;
        //private ICartRepository? _carts;

        private bool _disposed = false;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IStoreRepository Stores => _stores ??= new StoreRepository(_context);
        public IProductRepository Products => _products ??= new ProductRepository(_context);
        //public ICartRepository Carts => _carts ??= new CartRepository(_context);

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving changes to the database", ex);
            }
        }

        public int Save()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving changes to the database", ex);
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already active.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transactions to commit");

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally 
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!_disposed)
            {
                if(_transaction != null)
                    await _transaction.DisposeAsync();

                await _context.DisposeAsync();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}