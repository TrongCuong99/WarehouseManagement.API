using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}