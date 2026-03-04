using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.Shared;

namespace InterationTest
{
    public class SuppliersControllerTest(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region GetAllSuppliers Tests
        [Fact]
        public async Task GetAllSuppliers_ShouldReturnsOk_WhenSupplierExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier1 = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };

            var createSupplier2 = new CreateSupplierDto
            {
                Name = "Supplier B",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };

            Authenticate("Admin");
            await Client.PostAsJsonAsync("api/supplier", createSupplier1);
            await Client.PostAsJsonAsync("api/supplier", createSupplier2);

            // Act
            var response = await Client.GetAsync("api/supplier");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<SupplierDto>>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Data!.Count.Should().Be(2);
            Assert.NotEmpty(result!.Data!);
        }

        [Fact]
        public async Task GetAllSuppliers_ShouldReturnsEmptyList_WhenSupplierNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();

            // Act
            var response = await Client.GetAsync("api/supplier");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<SupplierDto>>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Data!.Should().BeEmpty();
        }
        #endregion

        #region GetSupplierById Tests
        [Fact]
        public async Task GetSupplierId_ShouldReturnsOk_WhenSupplierIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };

            Authenticate("Admin");
            var response = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var suppliersResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Act
            var result = await Client.GetAsync($"api/supplier/{suppliersResponse!.Data!.Id}");
            var resultContent = await result.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.NotNull(resultContent!.Data!);
            Assert.Equal("Get Supplier with Id Successful", resultContent.Message);
        }

        [Fact]
        public async Task GetSupplierId_ShouldReturnNotFound_WhenSupplierIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();

            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var response = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var suppliersResponse = await response.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Act
            var result = await Client.GetAsync($"api/supplier/{Guid.NewGuid}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region CreateSupplier Tests
        [Fact]
        public async Task CreateSupplier_ShouldReturnOk_WhenDataValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("api/supplier", createSupplier);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateSupplier_ShouldReturnConflict_WhenSupplierExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");

            // Act
            await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var response = await Client.PostAsJsonAsync("api/supplier", createSupplier);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateSupplier_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Staff");

            // Act
            await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var response = await Client.PostAsJsonAsync("api/supplier", createSupplier);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateSupplier_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var response = await Client.PostAsJsonAsync("api/supplier", createSupplier);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion

        #region DeleteSupplier Tests
        [Fact]
        public async Task DeleteSupplier_ShouldReturnOk_WhenSupplierIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Act
            var response = await Client.DeleteAsync($"api/supplier/{createdSupplier!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteSupplier_ShouldReturnNotFound_WhenSupplierIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Act
            var response = await Client.DeleteAsync($"api/supplier/{Guid.NewGuid}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteSupplier_ShouldReturnUnAuthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmailc.om",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var response = await Client.DeleteAsync($"api/supplier/{createdSupplier!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteSupplier_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();
            Authenticate("Staff");

            // Act
            var response = await Client.DeleteAsync($"api/supplier/{createdSupplier!.Data!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        #endregion

        [Fact]
        public async Task UpdateSupplier_ShouldReturnOk_WhenDataValid()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var updatesupp = new UpdateSupplierDto
            {
                Address = "456 New St",
                Name = "Supplier Updated",
                PhoneNumber = "987654321",
                ContactEmail = "update@gmail.com"
            };

            // Act
            var update = await Client.PutAsJsonAsync($"api/supplier/{createdSupplier!.Data!.Id}", updatesupp);
            var responseupdate = await update.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Assert
            update.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal("Supplier Updated", responseupdate!.Data!.Name);
            Assert.Equal("456 New St", responseupdate!.Data!.Address);
            Assert.Equal("987654321", responseupdate!.Data!.PhoneNumber);
            Assert.Equal("update@gmail.com", responseupdate!.Data!.ContactEmail);
        }

        [Fact]
        public async Task UpdateSupplier_ShouldReturnNotFound_WhenSupplierIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var updatesupp = new UpdateSupplierDto
            {
                Address = "456 New St",
                Name = "Supplier Updated",
                PhoneNumber = "987654321",
                ContactEmail = "update@gmail.com"
            };

            // Act
            var update = await Client.PutAsJsonAsync($"api/supplier/{Guid.NewGuid}", updatesupp);

            // Assert
            update.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateSupplier_ShouldReturnOk_WhenUpdatePartialData()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var updatesupp = new UpdateSupplierDto
            {
                Address = "456 New St",
                Name = "Supplier Updated",
            };

            // Act
            var update = await Client.PutAsJsonAsync($"api/supplier/{createdSupplier!.Data!.Id}", updatesupp);
            var responseupdate = await update.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            // Assert
            update.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.Equal("Supplier Updated", responseupdate!.Data!.Name);
            Assert.Equal("456 New St", responseupdate!.Data!.Address);
            Assert.Equal("123456789", responseupdate!.Data!.PhoneNumber);
            Assert.Equal("contact@gmail.com", responseupdate!.Data!.ContactEmail);
        }

        [Fact]
        public async Task UpdateSupplier_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var updatesupp = new UpdateSupplierDto
            {
                Address = "456 New St",
                Name = "Supplier Updated",
            };
            Authenticate("Staff");

            // Act
            var update = await Client.PutAsJsonAsync($"api/supplier/{createdSupplier!.Data!.Id}", updatesupp);

            // Assert
            update.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateSupplier_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createSupplier = new CreateSupplierDto
            {
                Name = "Supplier A",
                ContactEmail = "contact@gmail.com",
                PhoneNumber = "123456789",
                Address = "123 Main St"
            };
            Authenticate("Admin");
            var create = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var createdSupplier = await create.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();

            var updatesupp = new UpdateSupplierDto
            {
                Address = "456 New St",
                Name = "Supplier Updated",
            };
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");

            // Act
            var update = await Client.PutAsJsonAsync($"api/supplier/{createdSupplier!.Data!.Id}", updatesupp);

            // Assert
            update.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
