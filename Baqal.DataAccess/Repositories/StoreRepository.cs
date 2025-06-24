using Baqal.DataContext;
using Baqal.Entities.Models;

namespace Baqal.DataAccess.Repositories
{
    public class StoreRepository : BaseRepository<Store>
    {
        public StoreRepository(AppDbContext context) : base(context) { }
    }
}
