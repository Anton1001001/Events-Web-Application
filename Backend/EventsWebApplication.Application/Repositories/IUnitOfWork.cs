namespace EventsWebApplication.Application.Repositories;

public interface IUnitOfWork : IDisposable
{
    IEventRepository EventRepository { get; }
    IUserRepository UserRepository { get; }
    
    Task<int> SaveChangesAsync();
}