using Baqal.Entities.Enums;


namespace Baqal.Application.DTOs
{
    public class AddProductDTO
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public UnitType Unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid StoreId { get; set; }
        public int StockQuantity { get; set; }

    }
}
