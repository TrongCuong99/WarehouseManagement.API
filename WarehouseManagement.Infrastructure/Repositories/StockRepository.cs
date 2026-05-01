using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class StockRepository(WarehouseDbContext context) : BaseRepository<Stock>(context), IStockRepository
    {

        public async Task<IEnumerable<Stock>> GetStocksByProductAsync(int productId)
        {
            return await _dbSet.Where(p => p.ProductId == productId).ToListAsync();
        }

        public async Task<IEnumerable<Stock>> GetStocksByWarehouseAsync(int warehouseId)
        {
            return await _dbSet.Where(w => w.WarehouseId == warehouseId).ToListAsync();
        }
        public async Task<Stock?> GetStockByStockId(int stockId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Id == stockId);
        }
        public async Task<Stock?> GetStockByProductAndWarehouseAsync(int productId, int warehouseId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
        }
    }
}
