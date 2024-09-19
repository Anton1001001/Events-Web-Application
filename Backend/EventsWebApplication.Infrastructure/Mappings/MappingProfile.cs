using AutoMapper;
using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Infrastructure.DbEntities;

namespace EventsWebApplication.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<UserResponseDto, User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())  // Skip PasswordHash
            .ForMember(dest => dest.Role, opt => opt.Ignore()); 
        
        CreateMap<EventRequestDto, Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ConstructUsing(dto => new Event(
                Guid.NewGuid(),
                dto.Name, 
                dto.Description, 
                dto.DateTime, 
                dto.Location, 
                dto.Category, 
                dto.MaxUsers 
            ));

        CreateMap<User, UserViewDataDto>();
        
        CreateMap<Event, EventEntity>()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
            .ReverseMap()
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));
        
        CreateMap<User, UserEntity>().ReverseMap();

        CreateMap<RefreshTokenEntity, RefreshTokenDto>().ReverseMap();
        
        CreateMap<Event, EventEntity>().ReverseMap();
        CreateMap<Event, EventResponseDto>().ReverseMap();


    }
    
}