using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Entities;

namespace InterationTest
{
    public class WarehousesControllerTest(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region CreateWarehouse Test
        [Fact]
        public async Task CreateWarehouse_ShouldReturnsOk_WhenDataValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            // Act
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var createdWarehouse = await createWarehouseResponse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Assert
            createWarehouseResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(createdWarehouse);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldReturnsConflict_WhenNameWarehouseExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 3000,
                UserId = userId!.Data!.Id,
            };

            // Act
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);

            // Assert
            createWarehouseResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldReturnsNotFound_WhenUserIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = Guid.NewGuid(),
            };

            // Act
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);

            // Assert
            createWarehouseResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldReturnsUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);

            // Assert
            createWarehouseResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateWarehouse_ShouldReturnsForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };
            Authenticate("Staff");

            // Act
            var createWarehouseResponse = await Client.PostAsJsonAsync("api/warehouse", warehouse);

            // Assert
            createWarehouseResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region GetAllWarehouse Test
        [Fact]
        public async Task GetAllWarehouse_ShouldReturnsOk_WhenWarehouseExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Test Location",
                Capacity = 2000,
                UserId = userId!.Data!.Id,
            };
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            await Client.PostAsJsonAsync("api/warehouse", warehouse);

            // Act
            var getAllWarehouse = await Client.GetAsync("api/warehouse");
            var getAllWarehouseResponse = await getAllWarehouse.Content.ReadFromJsonAsync<ApiResponse<List<WarehouseDto>>>();

            // Assert
            getAllWarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAllWarehouseResponse.Should().NotBeNull();
            getAllWarehouseResponse!.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllWarehouse_ShouldReturnsOk_WhenWarehouseNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

            // Act
            var getAllWarehouse = await Client.GetAsync("api/warehouse");
            var getAllWarehouseResponse = await getAllWarehouse.Content.ReadFromJsonAsync<ApiResponse<List<WarehouseDto>>>();

            // Assert
            getAllWarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAllWarehouseResponse.Should().NotBeNull();
        }
        #endregion

        #region GetWarehouseById Test
        [Fact]
        public async Task GetWarehouseById_ShouldReturnsOk_WhenWarehouseIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Test Location",
                Capacity = 2000,
                UserId = userId!.Data!.Id,
            };
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Act
            var getAllWarehouse = await Client.GetAsync($"api/warehouse/{WarehouseId!.Data!.Id}");
            var getAllWarehouseResponse = await getAllWarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Assert
            getAllWarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAllWarehouseResponse.Should().NotBeNull();
            Assert.Equal(warehouse.Name, getAllWarehouseResponse!.Data!.Name);
            Assert.Equal(warehouse.Location, getAllWarehouseResponse!.Data!.Location);
            Assert.Equal(warehouse.Capacity, getAllWarehouseResponse!.Data!.Capacity);
            Assert.Equal(warehouse.UserId, getAllWarehouseResponse!.Data!.UserId);
        }

        [Fact]
        public async Task GetWarehouseById_ShouldReturnsNotFound_WhenWarehouseIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Test Location",
                Capacity = 2000,
                UserId = userId!.Data!.Id,
            };
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            await Client.PostAsJsonAsync("api/warehouse", warehouse);

            // Act
            var getAllWarehouse = await Client.GetAsync($"api/warehouse/{Guid.NewGuid}");

            // Assert
            getAllWarehouse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region UpdateWarehouse Test
        [Fact]
        public async Task UpdateWarehouse_ShouldReturnsOk_WhenWarehouseIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Test Location",
                Capacity = 2000,
                UserId = userId!.Data!.Id,
            };
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Updated Warehouse",
                Location = "Updated Location",
                Capacity = 3000,
                UserId = userId!.Data!.Id,
            };

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{WarehouseId!.Data!.Id}", updatedto);
            var updateWarehouseResponse = await updateWarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            updateWarehouseResponse.Should().NotBeNull();
            Assert.Equal(updatedto.Name, updateWarehouseResponse!.Data!.Name);
            Assert.Equal(updatedto.Location, updateWarehouseResponse!.Data!.Location);
            Assert.Equal(updatedto.Capacity, updateWarehouseResponse!.Data!.Capacity);
            Assert.Equal(updatedto.UserId, updateWarehouseResponse!.Data!.UserId);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnsNotFound_WhenWarehouseIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Test Location",
                Capacity = 2000,
                UserId = userId!.Data!.Id,
            };
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Updated Warehouse",
                Location = "Updated Location",
                Capacity = 3000,
                UserId = userId!.Data!.Id,
            };

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{Guid.NewGuid}", updatedto);

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnsConflict_WhenWarehouseNameExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var warehousetest = new CreateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Test Location",
                Capacity = 2000,
                UserId = userId!.Data!.Id,
            };
            await Client.PostAsJsonAsync("api/warehouse", warehousetest);
            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Updated Location",
                Capacity = 3000,
                UserId = userId!.Data!.Id,
            };

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{WarehouseId!.Data!.Id}", updatedto);

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnsNotFound_WhenUserIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Updated Location",
                Capacity = 3000,
                UserId = Guid.NewGuid(),
            };

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{WarehouseId!.Data!.Id}", updatedto);

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnsOk_WhenUpdatePartialWarehouse()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Updated Location",
                Capacity = 1000,
                UserId = Guid.Empty,
            };

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{WarehouseId!.Data!.Id}", updatedto);
            var updateWarehouseResponse = await updateWarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            updateWarehouseResponse.Should().NotBeNull();
            Assert.Equal(updatedto.Name, updateWarehouseResponse!.Data!.Name);
            Assert.Equal(updatedto.Location, updateWarehouseResponse!.Data!.Location);
            Assert.Equal(warehouse.Capacity, updateWarehouseResponse!.Data!.Capacity);
            Assert.Equal(warehouse.UserId, updateWarehouseResponse!.Data!.UserId);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Updated Location",
                Capacity = 1000,
                UserId = Guid.Empty,
            };
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{WarehouseId!.Data!.Id}", updatedto);

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateWarehouse_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            var updatedto = new UpdateWarehouseDto
            {
                Name = "Warehouse",
                Location = "Updated Location",
                Capacity = 1000,
                UserId = Guid.Empty,
            };
            Authenticate("Staff");

            // Act
            var updateWarehouse = await Client.PutAsJsonAsync($"api/warehouse/{WarehouseId!.Data!.Id}", updatedto);

            // Assert
            updateWarehouse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        #region DeleteWarehouse Test
        [Fact]
        public async Task DeleteWarehouse_ShouldReturnOk_WhenWarehouseIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Act
            var deleteWarehouse = await Client.DeleteAsync($"api/warehouse/{WarehouseId!.Data!.Id}");
            var deleteWarehouseResponse = await deleteWarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Assert
            deleteWarehouse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal("Warehouse deleted successfully", deleteWarehouseResponse!.Message);
        }

        [Fact]
        public async Task DeleteWarehouse_ShouldReturnNotFound_WhenWarehouseIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();

            // Act
            var deleteWarehouse = await Client.DeleteAsync($"api/warehouse/{Guid.NewGuid}");

            // Assert
            deleteWarehouse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteWarehouse_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var deleteWarehouse = await Client.DeleteAsync($"api/warehouse/{WarehouseId!.Data!.Id}");

            // Assert
            deleteWarehouse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteWarehouse_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            Authenticate("Admin");

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

            var warehouse = new CreateWarehouseDto
            {
                Name = "Test Warehouse",
                Location = "Test Location",
                Capacity = 1000,
                UserId = userId!.Data!.Id,
            };

            var createwarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var WarehouseId = await createwarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();
            Authenticate("Staff");

            // Act
            var deleteWarehouse = await Client.DeleteAsync($"api/warehouse/{WarehouseId!.Data!.Id}");

            // Assert
            deleteWarehouse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion
    }
}
