using EventsWebApplication.Application.UseCases.Events.Commands.AddEvent;
using EventsWebApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Tests.UseCases.Events.Commands.AddEvent;

public static class TestDataFactory
{
    public static AddEventCommand CreateAddEventCommand(
        string name = "test")
    {
        return new AddEventCommand
        {
            Name = name,
        };
    }

    public static Event CreateEvent(AddEventCommand command)
    {
        return new Event
        {
            EventId = Guid.NewGuid(),
            Name = command.Name
        };
    }

    public static EventResponse CreateEventResponse(Event @event)
    {
        return new EventResponse
        {
            EventId = @event.EventId,
            Name = @event.Name
        };
    }
}