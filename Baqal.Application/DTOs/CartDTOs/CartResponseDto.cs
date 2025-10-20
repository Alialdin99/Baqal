using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Application.DTOs.CartDTOs
{
    public class CartResponseDto
    {
        public Guid CartId { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

}
