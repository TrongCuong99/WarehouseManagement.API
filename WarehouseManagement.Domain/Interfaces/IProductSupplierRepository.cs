using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IProductSupplierRepository : IRepository<ProductSupplier>
    {
        Task<ProductSupplier?> GetByProductAndSupplierAsync(Guid productId, Guid supplierId);
        Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<ProductSupplier>> GetBySupplierIdAsync(Guid supplierId);
        Task<decimal?> GetSupplyPriceAsync(Guid productId, Guid supplierId);
    }
}
