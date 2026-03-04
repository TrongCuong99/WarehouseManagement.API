using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class ProductSupplier : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public decimal SupplyPrice { get; set; }
        private ProductSupplier() { }

        public ProductSupplier(Guid productId, Guid supplierId, decimal supplyPrice)
        {
            if (productId == Guid.Empty)
                throw new DomainException("Invalid product ID.");
            if (supplierId == Guid.Empty)
                throw new DomainException("Invalid supplier ID.");
            if (supplyPrice < 0)
                throw new DomainException("Supply price cannot be negative.");

            ProductId = productId;
            SupplierId = supplierId;
            SupplyPrice = supplyPrice;
        }

        public void UpdateSupplyPrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new DomainException("Supply price cannot be negative.");
            SupplyPrice = newPrice;
        }
    }
}
