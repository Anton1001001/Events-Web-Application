using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Events.Queries.GetEventById;

public class GetEventByIdMappingProfile : Profile
{
    public GetEventByIdMappingProfile()
    {
        CreateMap<Event, EventResponse>().ForMember(dest => dest.AvailableSeats,
            opt => opt.Ignore());
    }
}