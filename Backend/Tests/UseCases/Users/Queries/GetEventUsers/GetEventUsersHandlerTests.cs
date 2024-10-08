using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.UseCases.Users.Queries.GetEventUsers;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Tests.UseCases.Users.Queries.GetEventUsers;

public class GetEventUsersHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg 
        => cfg.AddProfile<GetEventUsersMappingProfile>()));
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();

    public GetEventUsersHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_EventFound_ReturnsPagedUserList()
    {
        var request = TestDataFactory.CreateGetEventUsersQuery(pageNumber: 1, pageSize: 5);
        var @event = TestDataFactory.CreateEvent(request);
        var usersQuery = TestDataFactory.CreateUsersQuery(@event, count: 5);
        
        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(@event);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(usersQuery.BuildMock());
        
        var handler = new GetEventUsersHandler(_mapper, _unitOfWorkMock.Object);
        
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(5);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EventNotFound_ReturnsEventNotFoundError()
    {
        var request = TestDataFactory.CreateGetEventUsersQuery(pageNumber: 1, pageSize: 5);
        var @event = TestDataFactory.CreateEvent(request);
        var usersQuery = TestDataFactory.CreateUsersQuery(@event, count: 5);
        
        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Event);

        _unitOfWorkMock.Setup(u => u.EventRepository
                .GetAllUsersAsync(It.IsAny<Guid>()))
            .Returns(usersQuery.BuildMock());
        
        var handler = new GetEventUsersHandler(_mapper, _unitOfWorkMock.Object);
        
        var result = await handler.Handle(request, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<EventNotFoundError>();
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.EventRepository
            .GetAllUsersAsync(It.IsAny<Guid>()), Times.Never); 
    }
    
}
