using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories
{
    public class UserRepository(WarehouseDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        protected WarehouseDbContext WarehouseContext => (WarehouseDbContext)_context;

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Email == username);
        }

        public async Task<IEnumerable<WarehouseTransaction>> GetApprovedTransactionsAsync(Guid userId)
        {
            return await WarehouseContext.WarehouseTransactions!
                .Where(t => t.ApprovedBy == userId)
                .Include(t => t.WarehouseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<IEnumerable<WarehouseTransaction>> GetCreatedTransactionsAsync(Guid userId)
        {
            return await WarehouseContext.WarehouseTransactions!
                .Where(t => t.CreatedBy == userId)
                .Include(t => t.WarehouseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesByUserAsync(Guid userId)
        {
            var user = await WarehouseContext.Users!
                        .Include(u => u.Warehouses)
                        .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Warehouses ?? Enumerable.Empty<Warehouse>();
        }

        public async Task<bool> HasApprovedTransactionsAsync(Guid userId)
        {
            return await WarehouseContext.WarehouseTransactions!
                .AnyAsync(t => t.ApprovedBy == userId);
        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
