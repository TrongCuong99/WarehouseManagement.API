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
        public int Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public TransactionTypes TransactionType { get; set; }
        public string Status { get; set; } = string.Empty; // Pending / Approved / Rejected
        public int WarehouseId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
