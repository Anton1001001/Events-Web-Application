using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsMappingProfile : Profile
{
    public GetAllEventsMappingProfile()
    {
        CreateProjection<Event, EventResponse>().ForMember(dest => dest.AvailableSeats,
            opt => opt.MapFrom(e => e.MaxUsers - e.Users.Count));
    }
    
}