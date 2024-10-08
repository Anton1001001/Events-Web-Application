using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;

public class GetUserEventsMappingProfile : Profile
{
    public GetUserEventsMappingProfile()
    {
        CreateProjection<Event, EventResponse>()
            .ForMember(dest => dest.AvailableSeats, opt =>
                opt.MapFrom(src => src.MaxUsers - src.Users.Count));
    }
}