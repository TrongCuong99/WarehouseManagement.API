using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<IEnumerable<Stock>> GetStocksByProductAsync(int productId);
        Task<IEnumerable<Stock>> GetStocksByWarehouseAsync(int warehouseId);
        Task<Stock?> GetStockByStockId(int stockId);
        Task<Stock?> GetStockByProductAndWarehouseAsync(int productId, int warehouseId);
    }
}
