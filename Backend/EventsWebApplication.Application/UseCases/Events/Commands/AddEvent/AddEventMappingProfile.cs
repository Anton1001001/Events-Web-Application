using AutoMapper;
using EventsWebApplication.Domain.Entities;

namespace EventsWebApplication.Application.UseCases.Events.Commands.AddEvent;

public class AddEventMappingProfile : Profile
{
    public AddEventMappingProfile()
    {
        CreateMap<AddEventCommand, Event>().
            ForMember(dest => dest.ImageUrl, 
                opt => opt.Ignore());

        CreateMap<Event, EventResponse>().ForMember(dest => dest.AvailableSeats, opt
            => opt.MapFrom(_ => 0));
    }
}