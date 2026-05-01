using AutoMapper;
using WarehouseManagement.Application.Common.Extensions;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Domain.Entities;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.Application.Services
{
    public class SupplierService(IUnitOfWork unitOfWork, IMapper mapper) : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task AssignProductAsync(int supplierId, int productId)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdWithProductsAsync(supplierId)
                           ?? throw new KeyNotFoundException("Supplier not found.");

            var product = await _unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            if (!supplier.ProductSuppliers.Any(ps => ps.ProductId == productId))
            {
                supplier.ProductSuppliers.Add(new ProductSupplier(productId, supplierId, 0));
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            var existingSupplier = await _unitOfWork.Supplier.GetByNameAsync(dto.Name);

            if(existingSupplier != null)
            {
                throw new ConflictException("Supplier with the same name already exists.");
            }

            var supplier = new Supplier(
                dto.Name,
                dto.ContactEmail,
                dto.PhoneNumber,
                dto.Address
            );

            await _unitOfWork.Supplier.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task DeleteAsync(int supplierId)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId) ?? throw new KeyNotFoundException("Supplier not found.");

            _unitOfWork.Supplier.Delete(supplier);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            var suppliers = _unitOfWork.Supplier.GetAllAsync();
            var result = await suppliers.ToPagedListAsync(1, 10);
            return _mapper.Map<List<SupplierDto>>(result);
        }

        public async Task<SupplierDto?> GetByIdAsync(int supplierId)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId) ?? throw new KeyNotFoundException("Supplier with Id not found.");
            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<IEnumerable<ProductSimpleDto>> GetProductsBySupplierAsync(int supplierId)
        {
            var supplier = await _unitOfWork.Supplier.GetProductsBySupplierAsync(supplierId)
                            ?? throw new KeyNotFoundException("Supplier not found.");

            return supplier.Select(ps => new ProductSimpleDto
                {
                    Id = ps.Id,
                    Name = ps.Name
                });
        }

        public async Task RemoveProductAsync(int supplierId, int productId)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdWithProductsAsync(supplierId) ?? throw new KeyNotFoundException("Supplier not found.");

            var item = supplier.ProductSuppliers.FirstOrDefault(ps => ps.ProductId == productId);

            if (item != null)
            {
                supplier.ProductSuppliers.Remove(item);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<SupplierDto> UpdateAsync(int supplierId, UpdateSupplierDto dto)
        {
            var supplier = await _unitOfWork.Supplier.GetByIdAsync(supplierId) ?? throw new KeyNotFoundException("Supplier not found.");

            if (!string.IsNullOrEmpty(dto.Name))
                supplier.SetName(dto.Name);

            if (!string.IsNullOrEmpty(dto.ContactEmail))
                supplier.SetContactEmail(dto.ContactEmail);

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                supplier.SetPhoneNumber(dto.PhoneNumber);

            if (!string.IsNullOrEmpty(dto.Address))
                supplier.Address = dto.Address;

            supplier.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Supplier.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SupplierDto>(supplier);
        }
    }
}
