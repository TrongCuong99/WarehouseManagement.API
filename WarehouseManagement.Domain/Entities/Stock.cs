using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class Stock : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        private Stock() { }

        public Stock(Guid productId, Guid warehouseId, int reorderLevel)
        {
            if (productId == Guid.Empty)
                throw new DomainException("Invalid product ID.");
            if (warehouseId == Guid.Empty)
                throw new DomainException("Invalid warehouse ID.");
            if (reorderLevel < 0)
                throw new DomainException("Reorder level cannot be negative."); 

            ProductId = productId;
            WarehouseId = warehouseId;
            ReorderLevel = reorderLevel;
            QuantityOnHand = 0;
            LastUpdated = DateTime.UtcNow;
        }

        public void Increase(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Increase quantity must be positive.");

            QuantityOnHand += quantity;
            LastUpdated = DateTime.UtcNow;
        }

        public void Decrease(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Decrease quantity must be positive.");
            if (QuantityOnHand - quantity < 0)
                throw new DomainException("Not enough stock to decrease.");

            QuantityOnHand -= quantity;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateQuantity(int change)
        {
            if (change > 0)
                Increase(change);
            else if (change < 0)
                Decrease(-change);
        }

        public bool IsBelowReorderLevel() => QuantityOnHand < ReorderLevel;
    }
}
