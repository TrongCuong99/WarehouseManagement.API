using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Application.Shared;
using WarehouseManagement.Domain.Entities;

namespace InterationTest
{
    public class CreateDataTransactionValid(CustomWebApplicationFactory factory) : IntegrationTestBase(factory)
    {
        public async Task RegisterUser(string Email, string Password)
        {
            var register = new UserRegisterDto
            {
                Email = Email,
                Password = Password
            };
            await Client.PostAsJsonAsync("api/user/register", register);
        }

        public async Task<ApiResponse<UserDto>> LoginUser(string Email, string Password)
        {
            var login = new UserLoginDto
            {
                Email = Email,
                Password = Password
            };
            var response = await Client.PostAsJsonAsync("api/user/login", login);
            var token = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
            return token!;
        }

        public async Task<ApiResponse<WarehouseDto>> CreateWarehouse(string Name, string Location, int Capacity, Guid UserId)
        {
            Authenticate("Admin");
            var warehouse = new CreateWarehouseDto
            {
                Name = Name,
                Location = Location,
                Capacity = Capacity,
                UserId = UserId,
            };
            var createdWarehouse = await Client.PostAsJsonAsync("api/warehouse", warehouse);
            var createdWarehouseRes = await createdWarehouse.Content.ReadFromJsonAsync<ApiResponse<WarehouseDto>>();
            return createdWarehouseRes!;
        }

        public async Task<ApiResponse<CategoryDto>> CreateCategory(string Name, string Description)
        {

            var createCategoryDto = new CreateCategoryDto
            {
                Name = Name,
                Description = Description
            };

            var categories = await Client.PostAsJsonAsync("api/category", createCategoryDto);
            var categoriesResponse = await categories.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
            return categoriesResponse!;
        }

        public async Task<ApiResponse<SupplierDto>> CreateSuplier(string Name, string Contact, string PhoneNumber, string Add)
        {
            var createSupplier = new CreateSupplierDto
            {
                Name = Name,
                ContactEmail = Contact,
                PhoneNumber = PhoneNumber,
                Address = Add
            };

            var supplier = await Client.PostAsJsonAsync("api/supplier", createSupplier);
            var supplierResponse = await supplier.Content.ReadFromJsonAsync<ApiResponse<SupplierDto>>();
            return supplierResponse!;
        }

        public async Task<ApiResponse<ProductDto>> CreateProduct(string Name, string Description, decimal Price, string Sku, Guid CategoryId, Guid SupplierId)
        {
            var dto = new CreateProductDto
            {
                SKU = Sku,
                Name = Name,
                Description = Description,
                CategoryId = CategoryId,
                SupplierId = SupplierId,
                Price = Price
            };
            var createdProduct = await Client.PostAsJsonAsync("api/product", dto);
            var response = await createdProduct.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
            return response!;
        }
    }
}
