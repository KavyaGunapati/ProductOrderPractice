using AutoMapper;
using Models.DTOs;
using Entity=DataAccess.Entities;
namespace OrderPRoductAPI.Profiles
{
    public class Mappingprofile : Profile
    {
        public Mappingprofile()
        {
            CreateMap< Register, AuthResponse >().ForMember(
                dest => dest.UserId,
                opt => opt.MapFrom(src => src.Email)
            ).ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email)
            );
            CreateMap<Entity.Product, Product>();
            CreateMap<Product, Entity.Product>();
        }
    }
}