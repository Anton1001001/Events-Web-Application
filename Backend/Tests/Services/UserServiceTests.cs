using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Repositories;
using EventsWebApplication.Application.Services;
using EventsWebApplication.Domain.Entities;
using Moq;

namespace Tests.Services;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _unitOfWorkMock.SetupGet(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _userService = new UserService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RegisterUserForEventAsync_UserAlreadyRegistered_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var request = new RegisterUserForEventRequestDto
        {
            UserId = userId,
            EventId = eventId
        };

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(new User(userId, "test@example.com", "passwordHash"));
        _userRepositoryMock.Setup(repo => repo.RegisterInEventAsync(userId, eventId))
            .ReturnsAsync(true);

        var result = await _userService.RegisterUserForEventAsync(request);

        Assert.True(result);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserForEventAsync_UserNotRegistered_UpdatesUserAndRegisters_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var request = new RegisterUserForEventRequestDto
        {
            UserId = userId,
            EventId = eventId,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var user = new User(userId, "John", "Doe", new DateTime(1990, 1, 1), "passwordHash", "john.doe@example.com");

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(repo => repo.RegisterInEventAsync(userId, eventId))
            .ReturnsAsync(false);

        var result = await _userService.RegisterUserForEventAsync(request);

        Assert.False(result);
        _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserEventsAsync_ReturnsUserEvents()
    {
        var userId = Guid.NewGuid();
        var events = new List<Event>
        {
            new Event(Guid.NewGuid(), "Event 1", "Description 1", DateTime.Now, "Location 1", "Category 1", 100),
            new Event(Guid.NewGuid(), "Event 2", "Description 2", DateTime.Now, "Location 2", "Category 2", 50)
        };

        _userRepositoryMock.Setup(repo => repo.GetEventsAsync(userId)).ReturnsAsync(events);

        var result = await _userService.GetUserEventsAsync(userId);

        Assert.Equal(2, result.Count);
        Assert.Equal("Event 1", result.FirstOrDefault().Name);
    }

    [Fact]
    public async Task IsUserRegisteredForEventAsync_UserIsRegistered_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _userRepositoryMock.Setup(repo => repo.IsRegisteredForEventAsync(userId, eventId))
            .ReturnsAsync(true);

        var result = await _userService.IsUserRegisteredForEventAsync(userId, eventId);

        Assert.True(result);
    }

    [Fact]
    public async Task CancelUserRegistrationForEventAsync_ReturnsCancelledEventId()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        _userRepositoryMock.Setup(repo => repo.CancelRegistrationForEventAsync(userId, eventId))
            .ReturnsAsync(eventId);

        var result = await _userService.CancelUserRegistrationForEventAsync(userId, eventId);

        Assert.Equal(eventId, result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}