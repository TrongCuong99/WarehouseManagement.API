using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.WarehouseTransactions
{
    public class WarehouseTransactionDetailDto
    {
        public Guid Id { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;

        public Guid CreatedBy { get; set; }
        public string CreatedByEmail { get; set; } = string.Empty;

        public List<WarehouseTransactionItemDto> Items { get; set; } = [];
    }
}
