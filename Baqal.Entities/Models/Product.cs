using Baqal.Entities.Enums;

namespace Baqal.Entities.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public int Price { get; set; }
        public UnitType unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; }

    }
}
