using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Net.Http.Json;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.DTOs.WarehouseTransactions;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Enums;

namespace InterationTest
{
    public class TransactionsControllerTest(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region CreateTransaction Test
        [Fact]
        public async Task CreateTransaction_ShouldReturnsOk_WhenDataValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(transactionres);
            Assert.Equal("Create Transaction Successfully", transactionres.Message);
            Assert.NotNull(transactionres.Data);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnConflict_WhenReferNumberExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
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

            // Act
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnKeyNotFound_WhenWarehouseIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = Guid.NewGuid(),
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnKeyNotFound_WhenProductIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
                    },
                    new() {
                        ProductId = Guid.NewGuid(),
                        Quantity = 20,
                        UnitPrice = 100
                    }
                ]
            };

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnConflict_WhenProductIdDuplicated()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
                    },
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 100
                    }
                ]
            };

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnKeyNotFound_WhenUserIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "TestRef",
                CreatedBy = Guid.NewGuid(),
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "TestRef",
                CreatedBy = Guid.NewGuid(),
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateTransaction_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            Authenticate("Staff");

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "TestRef",
                CreatedBy = Guid.NewGuid(),
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Assert
            createTransaction.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region ApproveTransaction Test
        [Fact]
        public async Task ApproveTransaction_ShouldReturnOk_WhenTransactionIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            var approve = await Client.PostAsync($"api/transactions/{transactionres!.Data!.Id}/approve", null);

            // Assert
            approve.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ApproveTransaction_ShouldReturnKeyNotFound_WhenTransactionIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            // Act
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            var approve = await Client.PostAsync($"api/transactions/{Guid.NewGuid}/approve", null);

            // Assert
            approve.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ApproveTransaction_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var approve = await Client.PostAsync($"api/transactions/{transactionres!.Data!.Id}/approve", null);

            // Assert
            approve.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ApproveTransaction_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            Authenticate("Staff");

            // Act
            var approve = await Client.PostAsync($"api/transactions/{transactionres!.Data!.Id}/approve", null);

            // Assert
            approve.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region RejectTransaction Test
        [Fact]
        public async Task RejectTransaction_ShouldReturnOk_WhenTransactionIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            var reason = JsonContent.Create("Test Reject");

            // Act
            var reject = await Client.PostAsync($"api/transactions/{transactionres!.Data!.Id}/reject", reason);

            // Assert
            reject.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RejectTransaction_ShouldReturnNotFound_WhenTransactionIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            var reason = JsonContent.Create("Test Reject");

            // Act
            var reject = await Client.PostAsync($"api/transactions/{Guid.NewGuid}/reject", reason);

            // Assert
            reject.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RejectTransaction_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            var reason = JsonContent.Create("Test Reject");

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var reject = await Client.PostAsync($"api/transactions/{transactionres!.Data!.Id}/reject", reason);

            // Assert
            reject.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RejectTransaction_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();
            var reason = JsonContent.Create("Test Reject");
            Authenticate("Staff");

            // Act
            var reject = await Client.PostAsync($"api/transactions/{transactionres!.Data!.Id}/reject", reason);

            // Assert
            reject.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region GetAllTransaction Test
        [Fact]
        public async Task GetAllTransaction_ShouldReturnOk_WhenTransactionsExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Act
            var getalltrans = await Client.GetAsync($"api/transactions");

            // Assert
            getalltrans.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllTransaction_ShouldReturnListEmpty_WhenTransactionsNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            // Act
            var getalltrans = await Client.GetAsync($"api/transactions");
            var getalltransres = await getalltrans.Content.ReadFromJsonAsync<ApiResponse<List<WarehouseTransactionDto>>>();

            // Assert
            getalltrans.StatusCode.Should().Be(HttpStatusCode.OK);
            getalltransres!.Data!.Should().BeEmpty();
        }
        #endregion

        #region GetTransactionById Test
        [Fact]
        public async Task GetTransactionById_ShouldReturnOk_WhenTransactionsExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Act
            var gettranid = await Client.GetAsync($"api/transactions/{transactionres!.Data!.Id}");
            var gettransidres = await gettranid.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Assert
            gettranid.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(gettransidres);
            Assert.Equal(transactionres.Data.Id, gettransidres!.Data!.Id);
            Assert.Equal(transactionres.Data.ReferenceNumber, gettransidres.Data.ReferenceNumber);
            Assert.Equal(transactionres.Data.TransactionType, gettransidres.Data.TransactionType);
            Assert.Equal(transactionres.Data.Status, gettransidres.Data.Status);
        }

        [Fact]
        public async Task GetTransactionById_ShouldReturnNotFound_WhenTransactionsNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            await Client.PostAsJsonAsync("api/transactions/create", transactiondto);

            // Act
            var gettranid = await Client.GetAsync($"api/transactions/{Guid.NewGuid}");

            // Assert
            gettranid.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region DeleteTransaction Test
        [Fact]
        public async Task DeleteTransactionById_ShouldReturnOk_WhenTransactionIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Act
            var gettranid = await Client.DeleteAsync($"api/transactions/{transactionres!.Data!.Id}");
            var gettransidres = await gettranid.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Assert
            gettranid.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal("Delete Transaction Successfully", gettransidres!.Message);
        }

        [Fact]
        public async Task DeleteTransactionById_ShouldReturnNotFound_WhenTransactionIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Act
            var gettranid = await Client.DeleteAsync($"api/transactions/{Guid.NewGuid}");

            // Assert
            gettranid.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteTransactionById_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var gettranid = await Client.DeleteAsync($"api/transactions/{transactionres!.Data!.Id}");

            // Assert
            gettranid.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteTransactionById_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var transactiontest = new CreateWarehouseTransactionDto
            {
                TransactionType = TransactionTypes.Inbound,
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "Test",
                CreatedBy = userId!.Data!.Id,
                Status = "Pending",
                TransactionDetails =
                [
                    new() {
                        ProductId = response!.Data!.Id,
                        Quantity = 20,
                        UnitPrice = 200
                    }
                ]
            };
            await Client.PostAsJsonAsync("api/transactions/create", transactiontest);
            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            Authenticate("Staff");

            // Act
            var gettranid = await Client.DeleteAsync($"api/transactions/{transactionres!.Data!.Id}");

            // Assert
            gettranid.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region UpdateTransaction Test
        [Fact]
        public async Task UpdateTransactionById_ShouldReturnOk_WhenTransactionIdValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var updatetransactiondto = new UpdateWarehouseTransactionDto
            {
                WarehouseId = Guid.NewGuid(),
                ReferenceNumber = "TestUpdate",
                Status = "Approved",
            };

            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Act
            var updatetrans = await Client.PutAsJsonAsync($"api/transactions/update/{transactionres!.Data!.Id}", updatetransactiondto);
            var updatetransres = await updatetrans.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Assert
            updatetrans.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(updatetransres);
            Assert.Equal(updatetransactiondto.WarehouseId, updatetransres!.Data!.WarehouseId);
            Assert.Equal(updatetransactiondto.ReferenceNumber, updatetransres.Data.ReferenceNumber);
            Assert.Equal(updatetransactiondto.Status, updatetransres.Data.Status);
        }

        [Fact]
        public async Task UpdateTransactionById_ShouldReturnNotFound_WhenTransactionIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var updatetransactiondto = new UpdateWarehouseTransactionDto
            {
                WarehouseId = Guid.NewGuid(),
                ReferenceNumber = "TestUpdate",
                Status = "Approved",
            };

            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Act
            var updatetrans = await Client.PutAsJsonAsync($"api/transactions/update/{Guid.NewGuid}", updatetransactiondto);

            // Assert
            updatetrans.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateTransactionById_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var updatetransactiondto = new UpdateWarehouseTransactionDto
            {
                WarehouseId = Guid.NewGuid(),
                ReferenceNumber = "TestUpdate",
                Status = "Approved",
            };

            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var updatetrans = await Client.PutAsJsonAsync($"api/transactions/update/{transactionres!.Data!.Id}", updatetransactiondto);

            // Assert
            updatetrans.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateTransactionById_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var updatetransactiondto = new UpdateWarehouseTransactionDto
            {
                WarehouseId = Guid.NewGuid(),
                ReferenceNumber = "TestUpdate",
                Status = "Approved",
            };

            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            Authenticate("Staff");

            // Act
            var updatetrans = await Client.PutAsJsonAsync($"api/transactions/update/{transactionres!.Data!.Id}", updatetransactiondto);

            // Assert
            updatetrans.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateTransactionById_ShouldReturnOk_WhenUpdatePartialTransaction()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            var create = new CreateDataTransactionValid(factory);

            await create.RegisterUser("test@gmail.com", "Test@123");
            var userId = await create.LoginUser("test@gmail.com", "Test@123");
            var createdWarehouse = await create.CreateWarehouse("Test Warehouse", "Test Location", 1000, userId!.Data!.Id);
            var categoriesResponse = await create.CreateCategory("Test Category", "This is a test category");
            var supplierResponse = await create.CreateSuplier("Test Supplier", "Test@gmail.com", "1234567890", "123 Test St");
            var response = await create.CreateProduct("Test Product", "This is a test product", 900, "TEST123", categoriesResponse!.Data!.Id, supplierResponse!.Data!.Id);

            //create transaction
            var transactiondto = new CreateWarehouseTransactionDto
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

            var updatetransactiondto = new UpdateWarehouseTransactionDto
            {
                WarehouseId = createdWarehouse!.Data!.Id,
                ReferenceNumber = "TestUpdate",
                Status = "Approved",
            };

            var createTransaction = await Client.PostAsJsonAsync("api/transactions/create", transactiondto);
            var transactionres = await createTransaction.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Act
            var updatetrans = await Client.PutAsJsonAsync($"api/transactions/update/{transactionres!.Data!.Id}", updatetransactiondto);
            var updatetransres = await updatetrans.Content.ReadFromJsonAsync<ApiResponse<WarehouseTransactionDto>>();

            // Assert
            updatetrans.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(updatetransres);
            Assert.Equal(updatetransactiondto.WarehouseId, transactiondto.WarehouseId);
            Assert.Equal(updatetransactiondto.ReferenceNumber, updatetransres!.Data!.ReferenceNumber);
            Assert.Equal(updatetransactiondto.Status, updatetransres.Data.Status);
        }
        #endregion
    }
}
