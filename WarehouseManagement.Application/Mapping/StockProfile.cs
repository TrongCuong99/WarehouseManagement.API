using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Stocks;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mapping
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<Stock, StockDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.WarehouseName,
                           opt => opt.MapFrom(src => src.Warehouse.Id));

            CreateMap<CreateStockDto, Stock>();

            CreateMap<UpdateStockDto, Stock>();

            CreateMap<Stock, StockViewModel>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.WarehouseName,
                           opt => opt.MapFrom(src => src.Warehouse.Id));
        }
    }
}
