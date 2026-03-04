using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Warehouses
{
    public class WarehouseWithStockDto
    {
        public Guid Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public List<StockItemDto> Stocks { get; set; } = [];
    }

    public class StockItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
