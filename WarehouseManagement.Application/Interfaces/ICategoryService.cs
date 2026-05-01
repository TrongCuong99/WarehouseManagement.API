using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Categories;

namespace WarehouseManagement.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<IQueryable<CategoryDto?>> GetAllCategoriesAsync();
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task DeleteCategoryAsync(int id);
    }
}
