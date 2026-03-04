using WarehouseManagement.Application.DTOs.Supplier;
using WarehouseManagement.Domain.Entities;
using AutoMapper;

namespace WarehouseManagement.Application.Mapping
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Supplier, SupplierDto>()
                .ForMember(dest => dest.Products,
                           opt => opt.MapFrom(src => src.ProductSuppliers.Select(ps => ps.Product)));
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>();
        }
    }
}
