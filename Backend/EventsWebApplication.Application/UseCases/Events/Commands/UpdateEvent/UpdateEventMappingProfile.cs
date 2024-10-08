using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Events.Commands.UpdateEvent;

public class UpdateEventMappingProfile : Profile
{
    public UpdateEventMappingProfile()
    {
        CreateMap<UpdateEventCommand, Event>();
        CreateMap<Event, EventResponse>()
            .ForMember(dest => dest.AvailableSeats, opt
                => opt.Ignore());
    }
}