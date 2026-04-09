using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Application.DTOs.WarehouseTransactions
{
    public class WarehouseTransactionDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public TransactionTypes TransactionType { get; set; }
        public string Status { get; set; } = string.Empty; // Pending / Approved / Rejected
        public Guid WarehouseId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
