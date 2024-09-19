using Microsoft.AspNetCore.Http;

namespace EventsWebApplication.Application.DTOs;

public class EventRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxUsers { get; set; }

    public string? ImageUrl { get; set; }
    public IFormFile? Image { get; set; }
}