using EventsWebApplication.Domain.Repositories;

namespace EventsWebApplication.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventsWebApplicationDbContext _context;
    private IEventRepository? _eventRepository;
    private IUserRepository? _userRepository;

    public UnitOfWork(EventsWebApplicationDbContext context)
    {
        _context = context;
    }
    public IEventRepository EventRepository => _eventRepository ??= new EventRepository(_context);
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }
 
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
