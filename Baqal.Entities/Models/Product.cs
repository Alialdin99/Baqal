using Baqal.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Baqal.Entities.Models
{
    internal class Product
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public int Price { get; set; }
        public UnitType unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; }

    }
}
