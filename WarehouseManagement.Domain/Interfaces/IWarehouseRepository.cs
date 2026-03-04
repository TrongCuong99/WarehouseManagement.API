using System.Xml.Linq;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IWarehouseRepository : IRepository<Warehouse>
    {
        Task<Warehouse?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Product>> GetProductsInWarehouseAsync(Guid warehouseId);
        Task<bool> HasProductsAsync(Guid warehouseId);
        Task<int> GetTotalStockQuantityAsync(Guid warehouseId);
        Task<bool> ExistsByIdAsync(Guid warehouseId);
    }
}