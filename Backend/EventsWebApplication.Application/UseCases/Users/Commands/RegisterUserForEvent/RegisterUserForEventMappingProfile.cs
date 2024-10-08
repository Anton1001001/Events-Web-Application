using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Users.Commands.RegisterUserForEvent;

public class RegisterUserForEventMappingProfile : Profile
{
    public RegisterUserForEventMappingProfile()
    {
        CreateMap<RegisterUserForEventCommand, User>()
            .ForMember(dest => dest.UserId, opt
                => opt.Ignore());
    }
}