using AutoMapper;
using WarehouseManagement.Application.DTOs.Products;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryIds,
                           opt => opt.MapFrom(src => src.Categories.Select(c => c.Id)))
                .ForMember(dest => dest.CategoryNames,
                           opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)));

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore())
                .ForMember(dest => dest.ProductSuppliers, opt => opt.Ignore())
                .ForMember(dest => dest.Stocks, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDetails, opt => opt.Ignore());

            CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Id)))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stocks.Sum(s => s.QuantityOnHand)));
        }
    }
}
