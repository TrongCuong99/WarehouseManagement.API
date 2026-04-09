using AutoMapper;
using StructureMap;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId) ?? throw new KeyNotFoundException("Category not found");

            var productExists = await _unitOfWork.Products.ExistsBySKUAsync(dto.SKU);
            if(productExists)
            {
                throw new ConflictException("Product with the same SKU already exists");
            }

            var productsupplierExists = await _unitOfWork.Supplier.ExistsByIdAsync(dto.SupplierId);
            if (!productsupplierExists)
            {
                throw new KeyNotFoundException("Supplier with ID not exist");
            }

            var product = new Product(dto.Name, dto.Description!, dto.SKU, dto.Price)
            {
                Description = dto.Description!,
                Name = dto.Name,
                SKU = dto.SKU,
                Price = dto.Price
            };
            await _unitOfWork.Products.AddAsync(product);

            product.Categories.Add(category);

            var productSupplier = new ProductSupplier(
                productId: product.Id,
                supplierId: dto.SupplierId,
                supplyPrice: dto.Price
            );
            product.ProductSuppliers.Add(productSupplier);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product with ID not exist");
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto?>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync(p => p.Categories);
            return _mapper.Map<IEnumerable<ProductDto?>>(products);
        }

        public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product with ID not exist");

            if (!string.IsNullOrWhiteSpace(dto.SKU))
                product.SKU = dto.SKU;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                product.Description = dto.Description;

            if (dto.Price > 0)
                product.Price = dto.Price;

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product with ID not exist");
            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
