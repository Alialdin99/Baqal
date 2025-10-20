namespace Baqal.Entities.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Cart Cart { get; set; } = default!;

        public Guid ProductId { get; set; } = default!;
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
