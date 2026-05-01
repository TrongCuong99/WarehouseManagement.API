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
        Task<ProductSupplier?> GetByProductAndSupplierAsync(int productId, int supplierId);
        Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(int productId);
        Task<IEnumerable<ProductSupplier>> GetBySupplierIdAsync(int supplierId);
        Task<decimal?> GetSupplyPriceAsync(int productId, int supplierId);
    }
}
