

namespace EventsWebApplication.Domain.Entities;
public class Event
{
    public Guid EventId { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public int MaxUsers { get; set; }
    public ICollection<User> Users { get; set; } = null!;
    public ICollection<EventUser> EventUsers { get; set; } = null!;
}