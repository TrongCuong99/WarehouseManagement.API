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
        Task<SupplierDto> UpdateAsync(int supplierId, UpdateSupplierDto dto);
        Task DeleteAsync(int supplierId);
        Task<SupplierDto?> GetByIdAsync(int supplierId);
        Task<List<SupplierDto>> GetAllAsync();
        Task AssignProductAsync(int supplierId, int productId);
        Task RemoveProductAsync(int supplierId, int productId);
        Task<IEnumerable<ProductSimpleDto>> GetProductsBySupplierAsync(int supplierId);
    }
}
