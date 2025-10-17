using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Entities.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public int CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
