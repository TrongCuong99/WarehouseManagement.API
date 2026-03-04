using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Products
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string CategoryName { get; set; } = null!;
        public string SupplierName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Categories { get; set; } = [];
        public int StockQuantity { get; set; }

    }
}
