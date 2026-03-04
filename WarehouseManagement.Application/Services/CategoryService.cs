using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var exists = await _unitOfWork.Categories.GetByNameAsync(dto.Name);

            if (exists != null)
                throw new ConflictException("Category name already exists");

            var category = new Category(dto.Name, dto.Description);

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category not found");
            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDto?>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto?>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category with ID not exist");
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = _unitOfWork.Categories.GetByIdAsync(id).Result ?? throw new KeyNotFoundException("Category with ID not exist");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                category.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                category.Description = dto.Description;
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
