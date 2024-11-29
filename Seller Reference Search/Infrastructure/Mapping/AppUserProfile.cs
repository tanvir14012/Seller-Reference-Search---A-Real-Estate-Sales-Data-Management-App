using AutoMapper;
using Seller_Reference_Search.Infrastructure.Data.Models;
using Seller_Reference_Search.Models;

namespace Seller_Reference_Search.Infrastructure.Mapping
{
    public class AppUserProfile: Profile
    {
        public AppUserProfile()
        {
            CreateMap<AppUser, AppUserDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<AppUserDto, AppUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName.TrimEnd()))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName.TrimEnd()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        }
    }
}
