namespace Baqal.Entities.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; } // // Nullable for guest

        public string? SessionId { get; set; } // For guests
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
