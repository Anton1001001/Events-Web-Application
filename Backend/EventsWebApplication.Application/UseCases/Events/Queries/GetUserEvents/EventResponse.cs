namespace EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;

public class EventResponse
{
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public int MaxUsers { get; set; }
    public int AvailableSeats { get; set; }
}