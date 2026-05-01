using WarehouseManagement.Application.DTOs.Stocks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IStockService
    {
        Task<Stock> UpdateStockAsync(int productId, int warehouseId, int quantityChange);
        Task<IQueryable<StockDto>> GetAllStocksAsync();
        Task<StockDto?> GetStockByIdAsync(int stockId);
        Task<Stock> CreateStock(int productId, int warehoueId);
    }
}
