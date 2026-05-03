using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> ExistsByUsernameAsync(string username);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<IEnumerable<Warehouse>> GetWarehousesByUserAsync(int userId);
        Task<IEnumerable<WarehouseTransaction>> GetCreatedTransactionsAsync(int userId);
        Task<IEnumerable<WarehouseTransaction>> GetApprovedTransactionsAsync(int userId);
        Task<bool> HasApprovedTransactionsAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task DeleteRefreshTokenAsync(int userId);
    }
}
