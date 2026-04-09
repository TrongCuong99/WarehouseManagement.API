using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using static WarehouseManagement.Domain.Common.DomainException;

namespace UnitTest
{
    public class CategoryServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();

        private readonly CategoryService _categoryService;

        public CategoryServiceTest()
        {
            _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        #region Create Category Tests
        [Fact]
        public async Task CreateCategory_ShouldReturnSuccess_WhenDataValid()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByNameAsync(createCategoryDto.Name));

            var category = new Category(createCategoryDto.Name, createCategoryDto.Description)
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description
            };
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = category.Name, Description = category.Description });

            // Act
            var result = await _categoryService.CreateCategoryAsync(createCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createCategoryDto.Name, result.Name);
            Assert.Equal(createCategoryDto.Description, result.Description);
        }

        [Fact]
        public async Task CreateCategory_ShouldThrowException_WhenCategoryExist()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            var category = new Category("Electronics", "Electronic devices and gadgets");
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByNameAsync(createCategoryDto.Name)).ReturnsAsync(category);

            //Act&&Assert
            await Assert.ThrowsAsync<ConflictException>(() => _categoryService.CreateCategoryAsync(createCategoryDto));
        }
        #endregion

        #region Delete Category Tests
        [Fact]
        public async Task DeleteCategory_ShouldReturnSuccess_WhenCategoryExist()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices and gadgets");

            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(category.Id)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = category.Name, Description = category.Description });

            // Act
            await _categoryService.DeleteCategoryAsync(category.Id);

            // Assert
            _categoryRepoMock.Verify(r => r.Delete(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_ShouldThrowException_WhenCategoryNotExist()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices and gadgets");

            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Category?)null);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = category.Name, Description = category.Description });

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoryService.DeleteCategoryAsync(category.Id));
        }
        #endregion

        #region Get All Category Tests
        [Fact]
        public async Task GetAllCategories_ShouldReturnSuccess_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new("Electronics", "Electronic devices and gadgets"),
                new("Furniture", "Home and office furniture")
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto?>>(It.IsAny<IEnumerable<Category>>()))
                .Returns([
                new() { Name = "Electronics", Description = "Electronic devices and gadgets"},
                new() { Name = "Furniture", Description = "Home and office furniture"}
            ]);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllCategories_ShouldEmpty_WhenCategoriesNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetAllAsync()).ReturnsAsync([]);
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto?>>(It.IsAny<IEnumerable<Category>>())).Returns([]);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region Get Category By Id Tests
        [Fact]
        public async Task GetCategoryById_ShouldReturnSuccess_WhenCategoryExist()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices and gadgets");
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(category.Id)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = category.Name, Description = category.Description });

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
        }

        [Fact]
        public async Task GetCategoryById_ShouldThrowException_WhenCategoryNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Category?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoryService.GetCategoryByIdAsync(Guid.NewGuid()));
        }
        #endregion

        #region Update Category Tests
        [Fact]
        public async Task UpdateCategory_ShouldThrowException_WhenCategoryIdNotExist()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices and gadgets");
            var updateCategoryDto = new UpdateCategoryDto
            {
                Name = "Updated Electronics",
                Description = "Updated description"
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Category?)null);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = category.Name, Description = category.Description });

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _categoryService.UpdateCategoryAsync(category.Id, updateCategoryDto));
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnSuccess_WhenDataValid()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices and gadgets");
            var updateCategoryDto = new UpdateCategoryDto
            {
                Name = "Updated Electronics",
                Description = "Updated description"
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(category.Id)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = updateCategoryDto.Name, Description = updateCategoryDto.Description });
            // Act
            var result = await _categoryService.UpdateCategoryAsync(category.Id, updateCategoryDto);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateCategoryDto.Name, result.Name);
            Assert.Equal(updateCategoryDto.Description, result.Description);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnSuccess_WhenUpdatePartialData()
        {
            // Arrange
            var category = new Category("Electronics", "Electronic devices and gadgets");
            var updateCategoryDto = new UpdateCategoryDto
            {
                Name = "Partially Updated Electronics"
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(c => c.GetByIdAsync(category.Id)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(new CategoryDto() { Name = updateCategoryDto.Name, Description = category.Description });

            // Act
            var result = await _categoryService.UpdateCategoryAsync(category.Id, updateCategoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateCategoryDto.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
        }
        #endregion
    }
}
