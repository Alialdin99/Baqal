using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.DataAccess.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);

    }
}
