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
    public class SupplierRepository(WarehouseDbContext context) : BaseRepository<Supplier>(context), ISupplierRepository
    {
        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            return await _dbSet.AnyAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<Product>> GetProductsBySupplierAsync(Guid supplierId)
        {
            return await _dbSet
                .Where(s => s.Id == supplierId)
                .SelectMany(s => s.ProductSuppliers)
                .Select(ps => ps.Product)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdWithProductsAsync(Guid supplierId)
        {
            return await _dbSet.Include(s => s.ProductSuppliers)
                .ThenInclude(ps => ps.Product)
                .FirstOrDefaultAsync(s => s.Id == supplierId);
        }
    }
}
