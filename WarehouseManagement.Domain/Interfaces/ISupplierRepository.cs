using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> GetByNameAsync(string name);
        Task<bool> ExistsByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId);
        Task<Supplier?> GetByIdWithProductsAsync(int supplierId);
    }
}
