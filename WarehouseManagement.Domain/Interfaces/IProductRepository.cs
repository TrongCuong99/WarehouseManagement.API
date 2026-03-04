using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetBySKUAsync(string sku);
        Task<bool> ExistsBySKUAsync(string sku);
        Task<IEnumerable<Product>> SearchAsync(string keyword);
        Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds);
    }
}