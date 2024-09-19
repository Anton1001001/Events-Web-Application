using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Domain.Entities;
using Moq;

namespace Tests.Services;

public class EventServiceTests
{
    private readonly EventService _eventService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEventRepository> _eventRepositoryMock;

    public EventServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _eventRepositoryMock = new Mock<IEventRepository>();

        _unitOfWorkMock.SetupGet(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
        _eventService = new EventService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnsEventsWithTotalCount()
    {
        var page = 1;
        var pageSize = 10;
        var title = "Event Title";
        var date = DateTime.Now;
        var category = "Category";
        var location = "Location";

        var events = new List<Event>
        {
            new Event(Guid.NewGuid(), "Event 1", "Description 1", DateTime.Now, "Location 1", "Category 1", 100),
            new Event(Guid.NewGuid(), "Event 2", "Description 2", DateTime.Now, "Location 2", "Category 2", 50)
        };

        var totalCount = events.Count;

        _eventRepositoryMock.Setup(repo => repo.GetAllAsync(page, pageSize, title, date, category, location))
            .ReturnsAsync((events, totalCount));

        var (result, count) = await _eventService.GetAllEventsAsync(page, pageSize, title, date, category, location);

        Assert.Equal(totalCount, count);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsEvent()
    {
        var eventId = Guid.NewGuid();
        var @event = new Event(eventId, "Event Name", "Description", DateTime.Now, "Location", "Category", 100);

        _eventRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
            .ReturnsAsync(@event);

        var result = await _eventService.GetEventByIdAsync(eventId);

        Assert.NotNull(result);
        Assert.Equal(@event.Id, result?.Id);
    }

    [Fact]
    public async Task GetEventByNameAsync_ReturnsEvent()
    {
        var eventName = "Event Name";
        var @event = new Event(Guid.NewGuid(), eventName, "Description", DateTime.Now, "Location", "Category", 100);

        _eventRepositoryMock.Setup(repo => repo.GetByNameAsync(eventName))
            .ReturnsAsync(@event);

        var result = await _eventService.GetEventByNameAsync(eventName);

        Assert.NotNull(result);
        Assert.Equal(@event.Name, result?.Name);
    }

    [Fact]
    public async Task AddEventAsync_CreatesEventAndReturnsId()
    {
        var @event = new Event(Guid.NewGuid(), "Event Name", "Description", DateTime.Now, "Location", "Category", 100);
        var eventId = Guid.NewGuid();

        _eventRepositoryMock.Setup(repo => repo.AddAsync(@event))
            .ReturnsAsync(eventId);

        var result = await _eventService.AddEventAsync(@event);

        Assert.Equal(eventId, result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_UpdatesEventAndReturnsId()
    {
        var @event = new Event(Guid.NewGuid(), "Event Name", "Description", DateTime.Now, "Location", "Category", 100);
        var eventId = @event.Id;

        _eventRepositoryMock.Setup(repo => repo.UpdateAsync(@event))
            .ReturnsAsync(eventId);

        var result = await _eventService.UpdateEventAsync(@event);

        Assert.Equal(eventId, result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetEventUsersAsync_ReturnsUsers()
    {
        var eventId = Guid.NewGuid();
        var users = new List<User>
        {
            new User(Guid.NewGuid(), "john.doe@example.com", "passwordHash"),
            new User(Guid.NewGuid(), "jane.doe@example.com", "passwordHash")
        };

        _eventRepositoryMock.Setup(repo => repo.GetUsersAsync(eventId))
            .ReturnsAsync(users);

        var result = await _eventService.GetEventUsersAsync(eventId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DeleteEventAsync_DeletesEventAndReturnsId()
    {
        var eventId = Guid.NewGuid();

        _eventRepositoryMock.Setup(repo => repo.DeleteAsync(eventId))
            .ReturnsAsync(eventId);

        var result = await _eventService.DeleteEventAsync(eventId);

        Assert.Equal(eventId, result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}