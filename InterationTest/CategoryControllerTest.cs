using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.Shared;

namespace InterationTest
{
    public class CategoryControllerTest(CustomWebApplicationFactory factory) : IntegrationTestBase(factory), IClassFixture<CustomWebApplicationFactory>
    {
        #region CreateCategories Tests
        [Fact]
        public async Task CreateCategories_ShouldReturnOk_WhenCategoryNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateCategories_ShouldReturnConflict_WhenCategoryExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new CreateCategoryDto
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            await Client.PostAsJsonAsync("/api/category", createcategory);
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task CreateCategories_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Staff");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateCategories_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        #endregion

        #region GetCategoryById Tests
        [Fact]
        public async Task GetCategoryById_ShouldReturnOk_WhenCategoryIdExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var result = await Client.GetAsync($"api/category/{createdCategory!.Data!.Id}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetCategoryById_ShouldReturnNotFound_WhenCategoryIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var result = await Client.GetAsync($"api/category/{Guid.NewGuid}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region GetAllCategories Tests
        [Fact]
        public async Task GetAllCategories_ShouldReturnOk_WhenCategoriesExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            var createcategory2 = new
            {
                Name = "Books",
                Description = "Various kinds of books"
            };
            Authenticate("Admin");

            // Act
            await Client.PostAsJsonAsync("/api/category", createcategory);
            await Client.PostAsJsonAsync("/api/category", createcategory2);

            var result = await Client.GetAsync("api/category");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllCategories_ShouldReturnListEmpty_WhenCategoriesNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();

            // Act
            var result = await Client.GetAsync("api/category");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var categories = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CategoryDto?>>>();
            categories!.Data!.Should().BeEmpty();
            categories.Message.Should().Be("Get All Category Successfully");
        }
        #endregion

        #region DeleteCategory Tests
        [Fact]
        public async Task DeleteCategory_ShouldReturnOk_WhenCategoryExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new CreateCategoryDto
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var result = await Client.DeleteAsync($"api/category/{createdCategory!.Data!.Id}");

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnNotFound_WhenCategoryNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            var result = await Client.DeleteAsync($"api/category/{Guid.NewGuid}");

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            Authenticate("Staff");
            var result = await Client.DeleteAsync($"api/category/{createdCategory!.Data!.Id}");

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };

            // Act
            Authenticate("Admin");
            var response = await Client.PostAsJsonAsync("api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();

            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");
            var result = await Client.DeleteAsync($"api/category/{createdCategory!.Data!.Id}");

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
        #endregion

        [Fact]
        public async Task UpdateCategory_ShouldReturnOk_WhenCategoryExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
            var updatecategory = new
            {
                Name = "Updated Electronics",
                Description = "Updated description"
            };
            var result = await Client.PutAsJsonAsync($"api/category/{createdCategory!.Data!.Id}", updatecategory);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultContent = await result.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
            Assert.Equal("Updated Electronics", resultContent!.Data!.Name);
            Assert.Equal("Updated description", resultContent.Data.Description);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNotFound_WhenCategoryIdNotExist()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var updatecategory = new
            {
                Name = "Updated Electronics",
                Description = "Updated description"
            };
            var result = await Client.PutAsJsonAsync($"api/category/{Guid.NewGuid}", updatecategory);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnForbidden_WhenUserNotAdmin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
            var updatecategory = new
            {
                Name = "Updated Electronics",
                Description = "Updated description"
            };
            Authenticate("Staff");
            var result = await Client.PutAsJsonAsync($"api/category/{createdCategory!.Data!.Id}", updatecategory);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnUnauthorize_WhenUserNotLogin()
        {
            // Arrange
            await ResetDatabaseAsync();
            var createcategory = new
            {
                Name = "Electronics",
                Description = "Electronic devices and gadgets"
            };
            Authenticate("Admin");

            // Act
            var response = await Client.PostAsJsonAsync("/api/category", createcategory);
            var createdCategory = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
            var updatecategory = new
            {
                Name = "Updated Electronics",
                Description = "Updated description"
            };
            Client.DefaultRequestHeaders.Authorization = null;
            Client.DefaultRequestHeaders.Remove("X-Test-Role");
            var result = await Client.PutAsJsonAsync($"api/category/{createdCategory!.Data!.Id}", updatecategory);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
