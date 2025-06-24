using Baqal.DataAccess.Interfaces;
using Baqal.DataContext;
using Baqal.Entities.Models;

namespace Baqal.DataAccess.Repositories
{
    public class StoreRepository : BaseRepository<Store>, IStoreRepository
    {
        public StoreRepository(AppDbContext context) : base(context) { }
    }
}
