using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class WarehouseTransactionDetail : BaseEntity
    {
        public Guid WarehouseTransactionId { get; set; }
        public WarehouseTransaction WarehouseTransaction { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Remarks { get; set; } = string.Empty;
        private WarehouseTransactionDetail() { }

        public WarehouseTransactionDetail(Guid productId, int quantity, decimal unitPrice, string remarks)
        {
            ProductId = productId;
            SetQuantity(quantity);
            SetUnitPrice(unitPrice);
            Remarks = remarks;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");
            Quantity = quantity;
        }

        public void SetUnitPrice(decimal price)
        {
            if (price < 0)
                throw new DomainException("Unit price cannot be negative.");
            UnitPrice = price;
        }

        internal void SetTransaction(WarehouseTransaction transaction)
        {
            WarehouseTransaction = transaction;
            WarehouseTransactionId = transaction.Id;
        }
    }
}
