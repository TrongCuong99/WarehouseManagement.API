using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Application.DTOs.WarehouseTransactions
{
    public class WarehouseTransactionViewModel
    {
        public Guid Id { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;

        public Guid WarehouseId { get; set; }
        public string WarehouseLocation { get; set; } = string.Empty;

        public Guid CreatedBy { get; set; }
        public string CreatedByEmail { get; set; } = string.Empty;

        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByEmail { get; set; }

        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public List<WarehouseTransactionDetailDto> TransactionDetails { get; set; } = [];
        public List<WarehouseTransactionItemDto> WarehouseTransactionItemDtos { get; set; } = [];
    }
}
