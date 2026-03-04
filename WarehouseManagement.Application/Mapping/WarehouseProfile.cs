using AutoMapper;
using WarehouseManagement.Application.DTOs.Warehouses;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mapping
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();

            CreateMap<CreateWarehouseDto, Warehouse>();
            CreateMap<UpdateWarehouseDto, Warehouse>();

            CreateMap<Warehouse, WarehouseWithStockDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Stocks, opt => opt.MapFrom(src => src.Stocks));

            CreateMap<Stock, StockItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.QuantityOnHand, opt => opt.MapFrom(src => src.QuantityOnHand));
        }
    }
}
