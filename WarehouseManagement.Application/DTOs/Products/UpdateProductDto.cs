using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Products
{
    public class UpdateProductDto
    {
        public string? SKU { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ReorderLevel { get; set; }
        public decimal Price { get; set; }
    }
}
