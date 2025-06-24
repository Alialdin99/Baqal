using Baqal.DataContext;
using Baqal.Entities.Models;


namespace Baqal.DataAccess.Repositories
{
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

    }
}
