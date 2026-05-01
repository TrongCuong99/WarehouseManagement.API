using System.Xml.Linq;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<Warehouse?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Product>> GetProductsInWarehouseAsync(int warehouseId);
        Task<bool> HasProductsAsync(int warehouseId);
        Task<int> GetTotalStockQuantityAsync(int warehouseId);
        Task<bool> ExistsByIdAsync(int warehouseId);
    }
}