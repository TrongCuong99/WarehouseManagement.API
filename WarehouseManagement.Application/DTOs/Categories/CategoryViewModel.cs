using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Products;

namespace WarehouseManagement.Application.DTOs.Categories
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductDto> Products { get; set; } = [];
    }
}
