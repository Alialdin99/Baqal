using Baqal.DataAccess.Interfaces;
using Baqal.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Baqal.DataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync(); ;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            
        }

        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;

        }

        public async Task DeleteAsync(int id)
        {
            var entity  = await GetByIdAsync(id);

            if(entity != null)
                _context.Set<T>().Remove(entity);
        }
       
    }
}
