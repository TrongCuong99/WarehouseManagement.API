using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductDto?>> GetAllProductsAsync();
        Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task DeleteProductAsync(Guid id);
    }
}
