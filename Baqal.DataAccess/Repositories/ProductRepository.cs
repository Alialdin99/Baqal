using Baqal.DataAccess.Interfaces;
using Baqal.DataContext;
using Baqal.Entities.Models;


namespace Baqal.DataAccess.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

    }
}
