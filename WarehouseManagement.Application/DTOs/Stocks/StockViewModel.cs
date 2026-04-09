using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Stocks
{
    public class StockViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
