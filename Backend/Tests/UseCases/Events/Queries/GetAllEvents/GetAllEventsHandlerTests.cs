using AutoMapper;
using EventsWebApplication.Application.UseCases.Events.Queries.GetAllEvents;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Tests.UseCases.Events.Queries.GetAllEvents;

public class GetAllEventsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEventRepository> _eventRepositoryMock = new();
    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<GetAllEventsMappingProfile>()));

    public GetAllEventsHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.EventRepository).Returns(_eventRepositoryMock.Object);
    }
    

    [Fact]
    public async Task Handle_ValidRequest_ReturnsPagedResult()
    {
        // Arrange
        var request = TestDataFactory.CreateGetAllEventsQuery(pageSize: 5);
        var eventsQuery = TestDataFactory.CreateEventsQuery(count: 4);

        _unitOfWorkMock.Setup(u => u.EventRepository.GetAllAsync())
            .Returns(eventsQuery.BuildMock());
        
        var handler = new GetAllEventsHandler(_unitOfWorkMock.Object, _mapper);
        
        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(4);
    }
}