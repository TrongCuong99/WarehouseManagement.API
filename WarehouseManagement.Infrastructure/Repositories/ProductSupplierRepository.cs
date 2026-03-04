using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class ProductSupplierRepository(DbContext context) : BaseRepository<ProductSupplier>(context), IProductSupplierRepository
    {
        public async Task<ProductSupplier?> GetByProductAndSupplierAsync(Guid productId, Guid supplierId)
        {
            return await _dbSet.FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.SupplierId == supplierId);
        }

        public async Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(Guid productId)
        {
            return await _dbSet.Where(ps => ps.ProductId == productId).ToListAsync();
        }

        public async Task<IEnumerable<ProductSupplier>> GetBySupplierIdAsync(Guid supplierId)
        {
            return await _dbSet.Where(ps => ps.SupplierId == supplierId).ToListAsync();
        }

        public Task<decimal?> GetSupplyPriceAsync(Guid productId, Guid supplierId)
        {
            return _dbSet
                .Where(ps => ps.ProductId == productId && ps.SupplierId == supplierId)
                .Select(ps => (decimal?)ps.SupplyPrice)
                .FirstOrDefaultAsync();
        }
    }
}
