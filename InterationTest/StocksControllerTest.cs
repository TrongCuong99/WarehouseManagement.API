using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Stocks;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Enums;

namespace InterationTest
{
    public class StocksControllerTes(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region Get All Stocks
        [Fact]
        public async Task GetAllStocks_ShouldReturnsOk_WhenNoStockExists()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            // Act
            var response = await Client.GetAsync("api/stock");
            var content = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<Stock>>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(content);
            Assert.Empty(content!.Data!);
        }

        [Fact]
        public async Task GetAllStocks_ShouldReturnOk_WhenStocksExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            //create user
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
            var userlogin = await Client.PostAsJsonAsync("api/user/login", request);
            var userId = await userlogin.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

            //create warehouse
            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var createdWarehouse = await createWarehouseResponse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            //create product
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
            var createdProduct = await Client.PostAsJsonAsync("api/product", dto);
            var response = await createdProduct.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            //create transaction
            var transaction = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id, 
                ReferenceNumber = "TestRef",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 10,
                        UnitPrice = 100
                    }
                ]
            };

            var createResponse = await Client.PostAsJsonAsync("api/transactions/create", transaction);
            var transactionId = await createResponse.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            await Client.PostAsync($"api/transactions/{transactionId!.Data!.Id}/approve", null);

            // Act
            var transactionwarehouse = await Client.GetAsync($"api/stock");
            var content = await transactionwarehouse.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<StockDto>>>();

            // Assert
            transactionwarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(content);
        }
        #endregion

        #region Get Stock By Id
        [Fact]
        public async Task GetStockById_ShouldReturnNotFound_WhenStockDoesNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");
            var StockId = Guid.NewGuid();

            // Act
            var response = await Client.GetAsync($"api/stock/{StockId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetStockById_ShouldReturnsOk_WhenStocksExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            //create user
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
            var userlogin = await Client.PostAsJsonAsync("api/user/login", request);
            var userId = await userlogin.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();

            //create warehouse
            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var createdWarehouse = await createWarehouseResponse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            //create product
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
            var response = await Client.PostAsJsonAsync("api/product", dto);
            var createdProduct = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();

            //create transaction
            var transaction = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "TestRef",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = createdProduct!.Data!.Id,
                        Quantity = 10,
                        UnitPrice = 100
                    }
                ]
            };

            var createResponse = await Client.PostAsJsonAsync("api/transactions/create", transaction);
            var transactionId = await createResponse.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            await Client.PostAsync($"api/transactions/{transactionId!.Data!.Id}/approve", null);

            // Act
            var stockId = await Client.GetAsync($"api/stock");
            var getstckId = await stockId.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<StockDto>>>();
            var firstStock = getstckId!.Data!.ToList();

            var stock = await Client.GetAsync($"api/stock/{firstStock[0].Id}");
            var content = await stock.Content.ReadFromJsonAsync<ApiResponse<StockDto>>();

            // Assert
            stock.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(content);
        }
        #endregion
    }
}
