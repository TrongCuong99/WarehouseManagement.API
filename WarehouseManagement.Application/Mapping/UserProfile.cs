using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Application.DTOs.Users;
using WarehouseManagement.Domain.Entities;

namespace WarehouseManagement.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<User, UserViewModel>();

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User"));

            CreateMap<UpdateUserDto, User>();
        }
    }
}
