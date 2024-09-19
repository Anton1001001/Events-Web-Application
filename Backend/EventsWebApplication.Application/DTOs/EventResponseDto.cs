namespace EventsWebApplication.Application.DTOs;

public class EventResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    public int AvailableSeats { get; set; }
    public int MaxUsers { get; set; }
}