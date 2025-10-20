using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Application.DTOs.CartDTOs
{
    public class CartItemResponseDto
    {
        public Guid ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!; // Join with Product table
        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}
