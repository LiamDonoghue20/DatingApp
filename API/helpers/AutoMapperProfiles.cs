using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        //map our data thats been formatted in our entities (AppUser and Photo)
        //into Dto's so the data is easier to manage
        {   
            CreateMap<AppUser, MemberDto>()
            //maps the main photo from app user to the member Dto
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDto>();
        }
    }
}