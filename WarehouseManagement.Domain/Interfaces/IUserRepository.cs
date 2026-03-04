using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> ExistsByUsernameAsync(string username);
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<IEnumerable<Warehouse>> GetWarehousesByUserAsync(Guid userId);
        Task<IEnumerable<WarehouseTransaction>> GetCreatedTransactionsAsync(Guid userId);
        Task<IEnumerable<WarehouseTransaction>> GetApprovedTransactionsAsync(Guid userId);
        Task<bool> HasApprovedTransactionsAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
