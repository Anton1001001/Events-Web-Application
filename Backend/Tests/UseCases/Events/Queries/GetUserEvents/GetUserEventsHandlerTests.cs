using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.UseCases.Events.Queries.GetUserEvents;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Tests.UseCases.Events.Queries.GetUserEvents;

public class GetUserEventsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly IMapper _mapper = new Mapper(
        new MapperConfiguration(cfg => cfg.AddProfile<GetUserEventsMappingProfile>()));

    public GetUserEventsHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserFound_ReturnsPagedUserEvents()
    {
        var request = TestDataFactory.CreateGetUserEventsQuery(pageNumber: 1, pageSize: 5);
        var user = TestDataFactory.CreateUser(request);
        var eventsQuery = TestDataFactory.CreateEventsQuery(count: 4);

        _unitOfWorkMock.Setup(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock.Setup(u => u.UserRepository
            .GetEvents(It.IsAny<Guid>())).Returns(eventsQuery.BuildMock());
        
        var handler = new GetUserEventsHandler(_unitOfWorkMock.Object, _mapper);
        
        var result = await handler.Handle(request, default);

        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(4);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.UserRepository.GetEvents(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        var request = TestDataFactory.CreateGetUserEventsQuery(pageNumber: 1, pageSize: 5);
        var user = TestDataFactory.CreateUser(request);
        var eventsQuery = TestDataFactory.CreateEventsQuery(count: 4);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);

        _unitOfWorkMock.Setup(u => u.UserRepository
            .GetEvents(It.IsAny<Guid>())).Returns(eventsQuery.BuildMock());
        
        var handler = new GetUserEventsHandler(_unitOfWorkMock.Object, _mapper);
        
        var result = await handler.Handle(request, default);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(u => u.UserRepository.GetEvents(It.IsAny<Guid>()), Times.Never);    
    }
    
}