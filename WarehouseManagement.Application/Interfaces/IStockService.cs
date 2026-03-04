using WarehouseManagement.Application.DTOs.Stocks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IStockService
    {
        Task<Stock> UpdateStockAsync(Guid productId, Guid warehouseId, int quantityChange);
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<StockDto?> GetStockByIdAsync(Guid stockId);
        Task<Stock> CreateStock(Guid productId, Guid warehoueId);
    }
}
