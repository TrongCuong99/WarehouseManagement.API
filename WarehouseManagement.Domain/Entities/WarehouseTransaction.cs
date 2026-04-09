using System.Linq;
using WarehouseManagement.Domain.Common;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Entities
{
    public class WarehouseTransaction : BaseEntity
    {
        public TransactionTypes TransactionType { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string ReferenceNumber { get; set; } = string.Empty;
        public Guid WarehouseId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public Guid? ApprovedBy { get; set; }
        public User? ApprovedByUser { get; set; }
        public string Status { get; private set; } = "Pending"; // Pending / Approved / Rejected
        public string? RejectionReason { get; private set; }
        public ICollection<WarehouseTransactionDetail> TransactionDetails { get; set; } = [];
        private WarehouseTransaction() { }

        public WarehouseTransaction(
            TransactionTypes transactionType,
            Guid warehouseId,
            Guid createdBy,
            string status,
            string referenceNumber)
        {
            SetTransactionType(transactionType);
            if (warehouseId == Guid.Empty)
                throw new DomainException("Warehouse is required.");
            if (createdBy == Guid.Empty)
                throw new DomainException("CreatedBy is required.");
            if (string.IsNullOrWhiteSpace(referenceNumber))
                throw new DomainException("Reference number is required.");

            WarehouseId = warehouseId;
            CreatedBy = createdBy;
            ReferenceNumber = referenceNumber.Trim();
            Status = status;
        }

        public void SetTransactionType(TransactionTypes type)
        {
            var allowed = new TransactionTypes[]
            {
                TransactionTypes.Inbound,
                TransactionTypes.Outbound,
                TransactionTypes.Adjustment,
                TransactionTypes.Pending
            };
            if (!allowed.Contains(type))
                throw new DomainException("Invalid transaction type.");
            TransactionType = type;
        }

        public void Approve(Guid? approvedBy)
        {
            if (Status == TransactionTypes.Approved.ToString())
                throw new DomainException("Transaction already approved.");

            ApprovedBy = approvedBy;
            Status = TransactionTypes.Approved.ToString();
            RejectionReason = null;
            UpdateAt = DateTime.UtcNow;
        }
        public void AddDetail(WarehouseTransactionDetail detail)
        {
            if (detail == null)
                throw new DomainException("Transaction detail cannot be null.");
            detail.SetTransaction(this);
            TransactionDetails.Add(detail);
        }
        public void Rejected(string reason)
        {
            if (Status == TransactionTypes.Rejected.ToString())
                throw new InvalidOperationException("Transaction already rejected");
            if (Status == TransactionTypes.Approved.ToString())
                throw new DomainException("Transaction alrealy approved cannot rejected.");

            RejectionReason = reason;
            Status = TransactionTypes.Rejected.ToString();
            ApprovedBy = null;
            UpdateAt = DateTime.UtcNow;
        }
        public void Pending()
        {
            Status = TransactionTypes.Pending.ToString();
            UpdateAt = DateTime.UtcNow;
        }
    }
}
