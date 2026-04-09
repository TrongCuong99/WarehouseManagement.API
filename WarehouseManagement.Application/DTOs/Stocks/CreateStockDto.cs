using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.Stocks
{
    public class CreateStockDto
    {
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
    }
}
