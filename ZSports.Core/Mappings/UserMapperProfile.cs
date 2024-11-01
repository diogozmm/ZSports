using AutoMapper;
using ZSports.Core.ViewModel.User;
using ZSports.Domain.User;
using ZSports.Keycloak.Request;

namespace ZSports.Core.Mappings
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile() 
        {
            CreateMap<UserViewModel, User>().ReverseMap();
            
            CreateMap<RegisterViewModel, User>().ReverseMap();
            
            CreateMap<UserViewModel, KeycloakRegisterUserRequest>().ReverseMap();
            
            CreateMap<RegisterViewModel, KeycloakRegisterUserRequest>().ReverseMap();
            
            CreateMap<LoginViewModel, KeycloakLoginUserRequest>()
                .ForMember(x => x.Username, src => src.MapFrom(x => x.Email))
                .ReverseMap();
        }
    }
}
