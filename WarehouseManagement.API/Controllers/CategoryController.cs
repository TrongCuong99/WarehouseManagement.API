using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(new ApiResponse<IEnumerable<CategoryDto?>>(200, "Get All Category Successfully", categories));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new ApiResponse<CategoryDto>(404, "Category with ID not exist"));
            }
            return Ok(new ApiResponse<CategoryDto>(200, "Get Category by Id Successfully", category));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateCategoryAsync(dto);
            if (category == null)
            {
                return BadRequest(new ApiResponse<CategoryDto>(400, "Create Category Failed"));
            }
            return Ok(new ApiResponse<CategoryDto>(200, "Create Category Successfully", category));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            if (category == null)
            {
                return BadRequest(new ApiResponse<CategoryDto>(400, "Update Category Failed"));
            }
            return Ok(new ApiResponse<CategoryDto>(200, "Update Category Successfully", category));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok(new ApiResponse<CategoryDto>(200, "Delete Category Successfully"));
        }
    }
}
