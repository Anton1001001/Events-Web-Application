using AutoMapper;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Infrastructure.DbEntities;
using EventsWebApplication.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Domain.Enums;
using EventsWebApplication.Infrastructure;


namespace Tests.Repositories;

public class UserRepositoryTests
{
    private readonly DbContextOptions<EventsWebApplicationDbContext> _dbContextOptions;
    private readonly IMapper _mapper;

    public UserRepositoryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserEntity>().ReverseMap();
            cfg.CreateMap<RefreshTokenDto, RefreshTokenEntity>().ReverseMap();
        });

        _mapper = config.CreateMapper();

        _dbContextOptions = new DbContextOptionsBuilder<EventsWebApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private EventsWebApplicationDbContext CreateContext()
    {
        return new EventsWebApplicationDbContext(_dbContextOptions);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUser()
    {
        using var context = CreateContext();

        var userToAdd = new User(Guid.NewGuid(), "test@example.com", "password");

        var repository = new UserRepository(context, _mapper);

        var addedUserId = await repository.AddAsync(userToAdd);
        await context.SaveChangesAsync();
        var userInDb = await context.Users.FindAsync(addedUserId);

        Assert.NotNull(userInDb);
        Assert.Equal("test@example.com", userInDb.Email);
        Assert.Equal(Role.Admin, userInDb.Role); // Default Role should be Admin as per the repository logic
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser()
    {
        using var context = CreateContext();

        var userId = Guid.NewGuid();
        var userEntity = new UserEntity
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "password",
            FirstName = "Test",
            LastName = "User",
            Role = Role.Admin
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mapper);

        var result = await repository.GetByEmailAsync("test@example.com");

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByRefreshTokenAsync_ShouldReturnUser()
    {
        using var context = CreateContext();

        var userId = Guid.NewGuid();
        var refreshToken = "refresh_token";
        var userEntity = new UserEntity
        {
            Id = userId,
            Email = "test@example.com",
            PasswordHash = "password",
            FirstName = "Test",
            LastName = "User",
            RefreshTokens = new List<RefreshTokenEntity>
            {
                new RefreshTokenEntity { Token = refreshToken, Expires = DateTime.UtcNow.AddDays(1) }
            }
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mapper);

        var result = await repository.GetByRefreshTokenAsync(refreshToken);

        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task RegisterInEventAsync_ShouldRegisterUserInEvent()
    {
        using var context = CreateContext();

        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();

        var userEntity = new UserEntity { Id = userId };
        context.Users.Add(userEntity);

        var repository = new UserRepository(context, _mapper);

        var alreadyRegistered = await repository.RegisterInEventAsync(userId, eventId);
        await context.SaveChangesAsync();

        var registration = await context.EventUsers
            .FirstOrDefaultAsync(eu => eu.UserId == userId && eu.EventId == eventId);

        Assert.NotNull(registration);
        Assert.False(alreadyRegistered); // should be false as it's the first time registering
    }

    [Fact]
    public async Task RemoveRefreshTokenAsync_ShouldRemoveToken()
    {
        using var context = CreateContext();

        var userId = Guid.NewGuid();
        var refreshToken = "refresh_token";
        var userEntity = new UserEntity
        {
            Id = userId,
            RefreshTokens = new List<RefreshTokenEntity>
            {
                new RefreshTokenEntity { Token = refreshToken, Expires = DateTime.UtcNow.AddDays(1) }
            }
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mapper);

        await repository.RemoveRefreshTokenAsync(userId, refreshToken);
        await context.SaveChangesAsync();

        var userInDb = await context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);

        Assert.NotNull(userInDb);
        Assert.DoesNotContain(userInDb.RefreshTokens, rt => rt.Token == refreshToken);
    }

    [Fact]
    public async Task RemoveExpiredRefreshTokensAsync_ShouldRemoveExpiredTokens()
    {
        using var context = CreateContext();

        var userId = Guid.NewGuid();
        var expiredToken = new RefreshTokenEntity
            { Token = "expired_token", Expires = DateTime.UtcNow.AddDays(-1) };
        var validToken = new RefreshTokenEntity { Token = "valid_token", Expires = DateTime.UtcNow.AddDays(1) };
        var userEntity = new UserEntity
        {
            Id = userId,
            RefreshTokens = new List<RefreshTokenEntity> { expiredToken, validToken }
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context, _mapper);

        await repository.RemoveExpiredRefreshTokensAsync(userId);
        await context.SaveChangesAsync();

        var userInDb = await context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);

        Assert.NotNull(userInDb);
        Assert.Single(userInDb.RefreshTokens);
        Assert.Equal("valid_token", userInDb.RefreshTokens.First().Token);
    }

}

