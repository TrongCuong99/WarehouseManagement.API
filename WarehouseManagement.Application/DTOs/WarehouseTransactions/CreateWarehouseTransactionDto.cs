using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Application.DTOs.WarehouseTransactions
{
    public class CreateWarehouseTransactionDto
    {
        public TransactionTypes TransactionType { get; set; }
        public Guid WarehouseId { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public required string Status { get; set; }

        public List<CreateWarehouseTransactionDetailDto> TransactionDetails { get; set; } = [];
    }

    public class CreateWarehouseTransactionDetailDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
