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
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
            //need to add configuration for the photo URL as the mapper wont automatically be able to locate it in the message interface to populate the DTO
                .ForMember(d => d.SenderPhotoUrl, o=> o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o=> o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}