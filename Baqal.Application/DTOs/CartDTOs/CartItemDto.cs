using System.ComponentModel.DataAnnotations;

namespace Baqal.Application.DTOs
{
    public class CartItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; } = default!;
        [Required]
        [Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999")]
        public int Quantity { get; set; }
    }
}