using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Entities.Models
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; } // // Nullable for guest

        public string? SessionId { get; set; } // For guests
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    }
}
