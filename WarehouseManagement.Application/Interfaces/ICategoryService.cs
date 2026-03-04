using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Categories;

namespace WarehouseManagement.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryDto?>> GetAllCategoriesAsync();
        Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task DeleteCategoryAsync(Guid id);
    }
}
