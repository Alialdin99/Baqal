using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Application.DTOs
{
    internal class CartItemDto
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
    }
}
