using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IStockRepository : IRepository<Stock>
    {
        Task<IEnumerable<Stock>> GetStocksByProductAsync(Guid productId);
        Task<IEnumerable<Stock>> GetStocksByWarehouseAsync(Guid warehouseId);
        Task<Stock?> GetStockByStockId(Guid stockId);
        Task<Stock?> GetStockByProductAndWarehouseAsync(Guid productId, Guid warehouseId);
    }
}
