using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class WarehouseRepository(WarehouseDbContext context) : BaseRepository<Warehouse>(context), IWarehouseRepository
    {
        private WarehouseDbContext WarehouseContext => (WarehouseDbContext)_context;
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(w => w.Name == name);
        }

        public async Task<Warehouse?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.Name == name);
        }

        public async Task<bool> ExistsByIdAsync(Guid warehouseId)
        {
            return await _dbSet.AnyAsync(w => w.Id == warehouseId);
        }

        public async Task<IEnumerable<Product>> GetProductsInWarehouseAsync(Guid warehouseId)
        {
            return await WarehouseContext.Stocks!
                .Where(s => s.WarehouseId == warehouseId)
                .Include(s => s.Product)
                .Select(s => s.Product)
                .ToListAsync();
        }

        public async Task<int> GetTotalStockQuantityAsync(Guid warehouseId)
        {
            return await WarehouseContext.Stocks!
                .Where(s => s.WarehouseId == warehouseId)
                .SumAsync(s => s.QuantityOnHand);
        }

        public async Task<bool> HasProductsAsync(Guid warehouseId)
        {
            return await WarehouseContext.Stocks!
                .AnyAsync(s => s.WarehouseId == warehouseId && s.QuantityOnHand > 0);
        }
    }
}
