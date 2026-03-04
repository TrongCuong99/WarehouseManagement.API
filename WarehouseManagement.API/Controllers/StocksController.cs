using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.DTOs.Stocks;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StocksController(IStockService stockService) : ControllerBase
    {
        private readonly IStockService _stockService = stockService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockService.GetAllStocksAsync();
            return Ok(new ApiResponse<IEnumerable<StockDto>>(200, "Get All Stocks Successfully", stocks));
        }

        [HttpGet("{stockId}")]
        public async Task<IActionResult> GetStockById(Guid stockId)
        {
            var stock = await _stockService.GetStockByIdAsync(stockId);
            if (stock == null)
            {
                return NotFound(new ApiResponse<StockDto>(404, "Not Found Stock with StockId"));
            }
            return Ok(new ApiResponse<StockDto>(200, "Get Stock by StockId Successfully", stock));
        }
    }
}
