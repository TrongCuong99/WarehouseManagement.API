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

        public async Task<IEnumerable<WarehouseTransaction>> GetApprovedTransactionsAsync(int userId)
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

        public async Task<IEnumerable<WarehouseTransaction>> GetCreatedTransactionsAsync(int userId)
        {
            return await WarehouseContext.WarehouseTransactions!
                .Where(t => t.CreatedBy == userId)
                .Include(t => t.WarehouseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesByUserAsync(int userId)
        {
            var user = await WarehouseContext.Users!
                        .Include(u => u.Warehouses)
                        .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Warehouses ?? Enumerable.Empty<Warehouse>();
        }

        public async Task<bool> HasApprovedTransactionsAsync(int userId)
        {
            return await WarehouseContext.WarehouseTransactions!
                .AnyAsync(t => t.ApprovedBy == userId);
        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task DeleteRefreshTokenAsync(int userId)
        {
            var user = await _dbSet.FindAsync(userId);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiresAt = null;
                _dbSet.Update(user);
            }
        }
    }
}
