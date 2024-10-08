using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;

public class GetEventUsersMappingProfile : Profile
{
    public GetEventUsersMappingProfile()
    {
        var eventId = Guid.Empty;
        CreateMap<User, UserResponse>().ForMember(dest => dest.RegistrationDate, opt
            => opt.MapFrom(src => src.EventUsers.First(eu => eu.EventId == eventId).RegistrationDate));
    }
}