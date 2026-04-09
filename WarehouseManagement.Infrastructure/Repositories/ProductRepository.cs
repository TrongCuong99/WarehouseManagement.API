using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class ProductRepository(WarehouseDbContext context) : BaseRepository<Product>(context), IProductRepository
    {
        public async Task<bool> ExistsBySKUAsync(string sku)
        {
            return await _dbSet.AnyAsync(p => p.SKU == sku);
        }

        public async Task<Product?> GetBySKUAsync(string sku)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            return await _dbSet.Where(p => p.Name.Contains(keyword) || p.SKU.Contains(keyword)).ToListAsync();
        }

        public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds)
        {
            return await _dbSet.Where(p => productIds.Contains(p.Id)).ToListAsync();
        }
    }
}
