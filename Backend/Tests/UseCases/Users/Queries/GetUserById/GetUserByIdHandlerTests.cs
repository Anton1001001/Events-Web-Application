using AutoMapper;
using EventsWebApplication.Application.Errors;
using EventsWebApplication.Application.UseCases.Users.Queries.GetUserById;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.UseCases.Users.Queries.GetUserById;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public GetUserByIdHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_UserFound_ReturnsMappedUserResponse()
    {
        var request = TestDataFactory.CreateGetUserByIdQuery();
        var user = TestDataFactory.CreateUser(request);
        var userResponse = TestDataFactory.CreateUserResponse(user);

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _mapperMock.Setup(m => m.Map<UserResponse>(It.IsAny<User>()))
            .Returns(userResponse);
        
        var handler = new GetUserByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        var result = await handler.Handle(request,default);
        
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Value.Should().BeEquivalentTo(userResponse);
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsUserNotFoundError()
    {
        var request = TestDataFactory.CreateGetUserByIdQuery();

        _unitOfWorkMock.Setup(u => u.UserRepository
                .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as User);
        
        var handler = new GetUserByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        
        var result = await handler.Handle(request,default);
        
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        
        _unitOfWorkMock.Verify(u => u.UserRepository
            .GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        
        _mapperMock.Verify(m => m
            .Map<UserResponse>(It.IsAny<User>()), Times.Never);
    }

}