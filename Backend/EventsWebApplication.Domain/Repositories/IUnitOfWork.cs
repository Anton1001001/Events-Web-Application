
namespace EventsWebApplication.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IEventRepository EventRepository { get; }
    IUserRepository UserRepository { get; }
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}