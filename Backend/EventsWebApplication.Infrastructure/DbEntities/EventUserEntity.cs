namespace EventsWebApplication.Infrastructure.DbEntities;

public class EventUserEntity
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public DateTime RegistrationDate { get; set; }
}