using EventsWebApplication.Application.UseCases.Events.Commands.DeleteEvent;
using EventsWebApplication.Domain.Entities;

namespace Tests.UseCases.Events.Commands.DeleteEvent;

public static class TestDataFactory
{
    public static DeleteEventCommand CreateDeleteEventCommand(
        Guid? eventId = null)
    {
        return new DeleteEventCommand
        {
            EventId = eventId ?? Guid.NewGuid()
        };
    }

    public static Event CreateEvent(DeleteEventCommand command)
    {
        return new Event
        {
            EventId = command.EventId
        };
    }
}