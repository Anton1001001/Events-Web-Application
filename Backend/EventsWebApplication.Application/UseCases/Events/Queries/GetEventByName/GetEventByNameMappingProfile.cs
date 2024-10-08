using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetEventByName;

public class GetEventByNameMappingProfile : Profile
{
    public GetEventByNameMappingProfile()
    {
        CreateMap<Event, EventResponse>().ForMember(dest => dest.AvailableSeats,
            opt => opt.Ignore());
    }
}