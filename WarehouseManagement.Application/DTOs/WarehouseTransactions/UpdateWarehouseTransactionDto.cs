using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagement.Application.DTOs.WarehouseTransactions
{
    public class UpdateWarehouseTransactionDto
    {
        public string Status { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
