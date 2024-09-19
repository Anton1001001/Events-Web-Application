namespace EventsWebApplication.Infrastructure.DbEntities;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserEntity UserEntity { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }


}