namespace Baqal.Entities.Models
{
    public class Store
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();


    }
}
