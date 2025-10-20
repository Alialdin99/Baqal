using Baqal.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baqal.Entities.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name {  get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public double Price { get; set; }
        public UnitType Unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid StoreId { get; set; }

        public int StockQuantity { get; set; }

    }
}
