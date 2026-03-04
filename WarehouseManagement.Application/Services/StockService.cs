using WarehouseManagement.Application.DTOs.Stocks;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class StockService(IUnitOfWork unitOfWork) : IStockService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Stock> CreateStock(Guid productId, Guid warehoueId)
        {
            var stock = new Stock(productId, warehoueId, 0) ?? throw new ConflictException("Failed to create stock");
            await _unitOfWork.Stocks.AddAsync(stock);
            return stock;
        }

        public async Task<Stock> UpdateStockAsync(Guid productId, Guid warehouseId, int quantityChange)
        {
            var stock = await _unitOfWork.Stocks.GetStockByProductAndWarehouseAsync(productId, warehouseId) ?? await CreateStock(productId, warehouseId);
            stock.UpdateQuantity(quantityChange);
            return stock;
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            var stocks = await _unitOfWork.Stocks.GetAllAsync(p => p.Product);
            return stocks.Select(stock => new StockDto
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                WarehouseId = stock.WarehouseId,
                QuantityOnHand = stock.QuantityOnHand,
                ProductName = stock.Product.Name
            });
        }

        public async Task<StockDto?> GetStockByIdAsync(Guid stockId)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(stockId, p => p.Product);
            if (stock == null)
            {
                throw new KeyNotFoundException("Stock not found");
            }
            return new StockDto
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                WarehouseId = stock.WarehouseId,
                QuantityOnHand = stock.QuantityOnHand,
                ProductName = stock.Product.Name
            };
        }
    }
}
