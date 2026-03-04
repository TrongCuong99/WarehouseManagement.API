using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Shared;

namespace WarehouseManagement.API.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductsController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(new ApiResponse<IEnumerable<ProductDto?>>(200, "Get All Products Successfully", products));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<ProductDto>(404, "Product with ID not exist"));
            }
            return Ok(new ApiResponse<ProductDto>(200, "Get Product by Id Successfully", product));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            if (product == null)
            {
                return BadRequest(new ApiResponse<ProductDto>(400, "Create Product Failed"));
            }
            return Ok(new ApiResponse<ProductDto>(200, "Create Product Successfully", product));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            if (product == null)
            {
                return BadRequest(new ApiResponse<ProductDto>(400, "Update Product Failed"));
            }
            return Ok(new ApiResponse<ProductDto>(200, "Update Product Successfully", product));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(new ApiResponse<ProductDto>(200, "Delete Product Successfully"));
        }
    }
}
