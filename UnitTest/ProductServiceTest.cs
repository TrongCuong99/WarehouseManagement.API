using AutoMapper;
using Moq;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Interfaces;
using static WarehouseManagement.Domain.Common.DomainException;

namespace UnitTest
{
    public class ProductServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<ICategoryRepository> _categoryRepoMock = new();

        private readonly ProductService _productService;
        public ProductServiceTest()
        {
            _productService = new ProductService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        #region CreateProduct Tests
        [Fact]
        public async Task CreateProduct_ShouldReturnSuccess_WhenDataValid()
        {
            //Arrange
            var createproduct = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TP001",
                Price = 100,
                CategoryId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(r => r.GetByIdAsync(createproduct.CategoryId)).ReturnsAsync(new Category("Dien tu", "do dung dien tu"));
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.ExistsBySKUAsync(createproduct.SKU)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(new Mock<ISupplierRepository>().Object);
            _unitOfWorkMock.Setup(u => u.Supplier.ExistsByIdAsync(createproduct.SupplierId)).ReturnsAsync(true);

            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(new ProductDto
            {
                Name = createproduct.Name,
                Description = createproduct.Description,
                Price = createproduct.Price
            });

            // Act
            var result = await _productService.CreateProductAsync(createproduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createproduct.Name, result.Name);
            Assert.Equal(createproduct.Description, result.Description);
            Assert.Equal(createproduct.Price, result.Price);

        }

        [Fact]
        public async Task CreateProduct_ShouldThrowException_WhenCategoryInCorrect()
        {
            //Arrange
            var createproduct = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TP001",
                Price = 100,
                CategoryId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(r => r.GetByIdAsync(createproduct.CategoryId)).ReturnsAsync((Category?)null);

            // Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _productService.CreateProductAsync(createproduct));

        }

        [Fact]
        public async Task CreateProduct_ShouldThrowException_ProductIdIsExist()
        {
            //Arrange
            var createproduct = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TP001",
                Price = 100,
                CategoryId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(r => r.GetByIdAsync(createproduct.CategoryId)).ReturnsAsync(new Category("Dien tu", "do dung dien tu"));
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.ExistsBySKUAsync(createproduct.SKU)).ReturnsAsync(true);

            //Act&&Assert
            await Assert.ThrowsAsync<ConflictException>(async () => await _productService.CreateProductAsync(createproduct));
        }

        [Fact]
        public async Task CreateProduct_ShouldThrowException_SupplierIdNotExist()
        {
            //Arrange
            var createproduct = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TP001",
                Price = 100,
                CategoryId = Guid.NewGuid(),
                SupplierId = Guid.NewGuid()
            };
            _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);
            _categoryRepoMock.Setup(r => r.GetByIdAsync(createproduct.CategoryId)).ReturnsAsync(new Category("Dien tu", "do dung dien tu"));
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.ExistsBySKUAsync(createproduct.SKU)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.Supplier).Returns(new Mock<ISupplierRepository>().Object);
            _unitOfWorkMock.Setup(u => u.Supplier.ExistsByIdAsync(createproduct.SupplierId)).ReturnsAsync(false);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _productService.CreateProductAsync(createproduct));

        }
        #endregion

        #region GetProductById Tests
        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            });
            // Act
            var result = await _productService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Price, result.Price);
        }

        [Fact]
        public async Task GetProductById_ShouldThrowException_WhenProductNotExists()
        {
            // Arrange
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync((Product?)null);
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _productService.GetProductByIdAsync(product.Id));
        }
        #endregion

        #region GetAllProducts Tests
        [Fact]
        public async Task GetAllProducts_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
            {
                new("Product 1", "Description 1", "SKU1", 50){Name = "Product 1", Description = "Description 1"},
                new("Product 2", "Description 2", "SKU2", 75){Name = "Product 2", Description = "Description 2"}
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetAllAsync(p => p.Categories)).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto?>>(products)).Returns(
            [
                new() { Name = "Product 1", Description = "Description 1", Price = 50 },
                new() { Name = "Product 2", Description = "Description 2", Price = 75 }
            ]);
            // Act
            var result = await _productService.GetAllProductsAsync();
            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowException_WhenProductsNotExist()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            //Act
            var result = await _productService.GetAllProductsAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        #endregion

        #region UpdateProduct Tests
        [Fact]
        public async Task UppdateProduct_ShouldReturnSuccess_WhenProductExist()
        {
            //Arrange
            var updateproduct = new UpdateProductDto {SKU = "TP002", Name = "Test Product1", Description = "Test Description1", Price = 200};
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto
            {
                Name = updateproduct.Name,
                Description = updateproduct.Description,
                Price = updateproduct.Price,
                SKU = updateproduct.SKU
            });

            //Act
            var result = await _productService.UpdateProductAsync(product.Id, updateproduct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(updateproduct.Name, result.Name);
            Assert.Equal(updateproduct.Price, result.Price);
            Assert.Equal(updateproduct.Description, result.Description);
            Assert.Equal(updateproduct.SKU, result.SKU);
        }

        [Fact]
        public async Task UppdateProduct_ShouldThrowException_WhenProductNotExist()
        {
            //Arrange
            var updateproduct = new UpdateProductDto { SKU = "TP002", Name = "Test Product1", Description = "Test Description1", Price = 200 };
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync((Product?)null);

            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _productService.UpdateProductAsync(product.Id, updateproduct));
        }

        [Fact]
        public async Task UppdateProduct_ShouldReturnSuccess_WhenUpdateOnlyProvidedFields()
        {
            //Arrange
            var updateproduct = new UpdateProductDto {Name = "Test Product1", Description = "Test Description1", Price = 100, SKU = "TP001"};
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto
            {
                Name = updateproduct.Name,
                Description = updateproduct.Description,
                Price = product.Price,
                SKU = product.SKU
            });

            //Act
            var result = await _productService.UpdateProductAsync(product.Id, updateproduct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(updateproduct.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(updateproduct.Description, result.Description);
            Assert.Equal(product.SKU, result.SKU);
        }

        [Fact]
        public async Task UppdateProduct_ShouldReturnSuccess_WhenUpdateEmptyStringFields()
        {
            //Arrange
            var updateproduct = new UpdateProductDto { Name = "Test Product1", SKU = "TP002" };
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto
            {
                Name = updateproduct.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = updateproduct.SKU
            });

            //Act
            var result = await _productService.UpdateProductAsync(product.Id, updateproduct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(updateproduct.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(updateproduct.SKU, result.SKU);
        }

        [Fact]
        public async Task UppdateProduct_ShouldNotUpdatePrice_WhenPriceIsInvalid()
        {
            //Arrange
            var updateproduct = new UpdateProductDto { Name = "Test Product1", SKU = "TP002", Description = "Test Description1", Price = -100 };
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto
            {
                Name = updateproduct.Name,
                Description = updateproduct.Description,
                Price = product.Price,
                SKU = updateproduct.SKU
            });

            //Act
            var result = await _productService.UpdateProductAsync(product.Id, updateproduct);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(updateproduct.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(updateproduct.Description, result.Description);
            Assert.Equal(updateproduct.SKU, result.SKU);
        }
        #endregion

        #region DeleteProduct Tests
        [Fact]
        public async Task DeleteProduct_ShouldReturnSuccess_WhenProductExist()
        {
            //Arrange
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU
            });

            //Act
            await _productService.DeleteProductAsync(product.Id);

            //Assert
            _productRepoMock.Verify(r => r.Delete(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowException_WhenProductNotExist()
        {
            //Arrange
            var product = new Product("Test Product", "Test Description", "TP001", 100)
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                SKU = "TP001"
            };
            _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _productRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((Product?)null);
            //Act&&Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _productService.DeleteProductAsync(product.Id));
        }
        #endregion
    }
}
