using Baqal.Entities.Enums;
using Baqal.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baqal.Application.DTOs
{
    public class UpdateProductDTO
    {

        public string Name { get; set; }
        public int Price { get; set; }
        public UnitType Unit { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
    }
}
