using AutoMapper;
using WarehouseManagement.Application.DTOs.Categories;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<CreateCategoryDto, Category>();

            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<Category, CategoryViewModel>()
                .ForMember(dest => dest.Products,
                           opt => opt.MapFrom(src => src.Products));
        }
    }
}
