using AutoMapper;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Infrastructure.Repositories;

namespace EventsWebApplication.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventsWebApplicationDbContext _context;
    private readonly IMapper _mapper;
    private IEventRepository? _eventRepository;
    private IUserRepository? _userRepository;

    public UnitOfWork(EventsWebApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEventRepository EventRepository
    {
        get
        {
            if (_eventRepository == null)
            {
                _eventRepository = new EventRepository(_context, _mapper);
            }
            return _eventRepository;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository = new UserRepository(_context, _mapper);
            }
            return _userRepository;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            this._disposed = true;
        }
    }
 
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
