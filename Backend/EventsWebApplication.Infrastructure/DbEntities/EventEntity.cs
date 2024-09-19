namespace EventsWebApplication.Infrastructure.DbEntities;

public class EventEntity
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; }
    public string Category { get; set; }
    public int MaxUsers { get; set; }
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    public string ImageUrl { get; set; }
    
}