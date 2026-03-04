using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Entities;

namespace InterationTest
{
    public class ProductsControllerTest(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region Create Product Tests
        [Fact]
        public async Task CreateProduct_ShouldReturnOk_WhenDataValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/login", request);
            Authenticate("Admin");

            await ResetDatabaseAsync();
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/product", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Create Product Successfully", responseString);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnConflict_WhenProductIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/login", request);
            Authenticate("Admin");

            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            // Act
            await Client.PostAsJsonAsync("api/product", dto);
            var createproduct = await Client.PostAsJsonAsync("api/product", dto);

            // Assert
            createproduct.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnNotFound_WhenCategoryIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/login", request);
            Authenticate("Admin");

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = Guid.NewGuid(),
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/product", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnNotFound_WhenSupplierIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/login", request);
            Authenticate("Admin");

            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = Guid.NewGuid(),
                Price = 900
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/product", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var request = new UserLoginDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/login", request);
            Authenticate("Admin");

            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = Guid.NewGuid(),
                Price = 900
            };
            Authenticate("Staff");

            // Act
            var response = await Client.PostAsJsonAsync("api/product", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var register = new UserRegisterDto
            {
                Email = "test@gmail.com",
                Password = "Test@123"
            };
            await Client.PostAsJsonAsync("api/user/register", register);

            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };
            Authenticate("Admin");

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = Guid.NewGuid(),
                Price = 900
            };
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var response = await Client.PostAsJsonAsync("api/product", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion

        #region Get Product By Id Tests
        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenProductExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            // Act
            var response = await Client.GetAsync($"api/product/{productResult!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNotFound_WhenProductNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            // Act
            var response = await Client.GetAsync($"api/product/{Guid.NewGuid}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region GetAll Product Tests
        [Fact]
        public async Task GetAllProduct_ShouldReturnOK_WhenProductExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto1 = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };
            var dto2 = new CreateProductDto
            {
                SKU = "TEST",
                Name = "Test Product2",
                Description = "This is a test product2",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 1000
            };


            await Client.PostAsJsonAsync("api/product", dto1);
            await Client.PostAsJsonAsync("api/product", dto2);

            // Act
            var response = await Client.GetAsync("api/product");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllProduct_ShouldReturnListEmpty_WhenProductNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();

            // Act
            var response = await Client.GetAsync("api/product");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductDto?>>>();
            result!.Data!.Should().BeEmpty();
        }
        #endregion

        #region Update Product Tests
        [Fact]
        public async Task UpdateProduct_ShouldReturnOk_WhenDataValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            var updatedto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Description = "Updated Description",
                Price = 1200
            };

            // Act
            var response = await Client.PutAsJsonAsync($"api/product/{productResult!.Data!.Id}", updatedto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto?>>();
            Assert.Equal("Update Product Successfully", result!.Message);
            Assert.Equal("Updated Product Name", result.Data!.Name);
            Assert.Equal("Updated Description", result.Data.Description);
            Assert.Equal(1200, result.Data.Price);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnOke_WhenUpdatePartialData()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            var updatedto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Description = "Updated Description",
                Price = 1200,
                SKU = "TEST23",
                ReorderLevel = 10
            };

            // Act
            var response = await Client.PutAsJsonAsync($"api/product/{productResult!.Data!.Id}", updatedto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto?>>();
            Assert.Equal("Update Product Successfully", result!.Message);
            Assert.Equal("Updated Product Name", result.Data!.Name);
            Assert.Equal("Updated Description", result.Data.Description);
            Assert.Equal(1200, result.Data.Price);
            Assert.Equal("TEST23", result.Data.SKU);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            var updatedto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Description = "Updated Description",
                Price = 1200,
                SKU = "TEST23",
                ReorderLevel = 10
            };

            Authenticate("Staff");
            // Act
            var response = await Client.PutAsJsonAsync($"api/product/{productResult!.Data!.Id}", updatedto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var updatedto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Description = "Updated Description",
                Price = 1200,
                SKU = "TEST23",
                ReorderLevel = 10
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PutAsJsonAsync($"api/product/{Guid.NewGuid}", updatedto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnUnAuthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };
            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            var updatedto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Description = "Updated Description",
                Price = 1200,
                SKU = "TEST23",
                ReorderLevel = 10
            };

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");
            // Act
            var response = await Client.PutAsJsonAsync($"api/product/{productResult!.Data!.Id}", updatedto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion

        #region Delete Product Tests
        [Fact]
        public async Task DeleteProduct_ShouldReturnOk_WhenProductIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            // Act
            var response = await Client.DeleteAsync($"api/product/{productResult!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenProductIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };

            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            // Act
            var response = await Client.DeleteAsync($"api/product/{Guid.NewGuid}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };
            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            Authenticate("Staff");
            // Act
            var response = await Client.DeleteAsync($"api/product/{productResult!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var createCategoryDto = new CreateCategoryDto
            {
                Name = "Test Category",
                Description = "This is a test category"
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Test Supplier",
                ContactEmail = "Test@gmail.com",
                PhoneNumber = "1234567890",
                Address = "123 Test St"
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var dto = new CreateProductDto
            {
                SKU = "TEST123",
                Name = "Test Product",
                Description = "This is a test product",
                CategoryId = categoriesResponse!.Data!.Id,
                SupplierId = supplierResponse!.Data!.Id,
                Price = 900
            };
            var productResponse = await Client.PostAsJsonAsync("api/product", dto);
            var productResult = await productResponse.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");
            // Act
            var response = await Client.DeleteAsync($"api/product/{productResult!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion
    }
}
