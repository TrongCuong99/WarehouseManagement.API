using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Application.DTOs.Supplier;

namespace WarehouseManagement.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
        Task<SupplierDto> UpdateAsync(Guid supplierId, UpdateSupplierDto dto);
        Task DeleteAsync(Guid supplierId);
        Task<SupplierDto?> GetByIdAsync(Guid supplierId);
        Task<List<SupplierDto>> GetAllAsync();
        Task AssignProductAsync(Guid supplierId, Guid productId);
        Task RemoveProductAsync(Guid supplierId, Guid productId);
        Task<IEnumerable<ProductSimpleDto>> GetProductsBySupplierAsync(Guid supplierId);
    }
}
