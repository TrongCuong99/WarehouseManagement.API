using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Products
{
    public class CreateProductDto
    {
        public string SKU { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }
        public decimal Price { get; set; }
    }
}
