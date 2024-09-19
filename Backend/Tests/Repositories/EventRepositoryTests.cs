using AutoMapper;
using EventsWebApplication.Domain.Entities;
using EventsWebApplication.Infrastructure.DbEntities;
using EventsWebApplication.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EventsWebApplication.Infrastructure;


namespace Tests.Repositories
{
    public class EventRepositoryTests
    {
        private readonly DbContextOptions<EventsWebApplicationDbContext> _dbContextOptions;
        private readonly IMapper _mapper;

        public EventRepositoryTests()
        {
            var config = new MapperConfiguration(cfg => { cfg.CreateMap<Event, EventEntity>().ReverseMap(); });

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
        public async Task GetAllAsync_ShouldReturnFilteredResults()
        {
            await using var context = CreateContext();

            var events = new List<EventEntity>
            {
                new EventEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Event",
                    Description = "Description 1",
                    DateTime = DateTime.Now,
                    Category = "Music",
                    Location = "New York",
                    MaxUsers = 100,
                    ImageUrl = "imageUrl 1"
                },
                new EventEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Event",
                    Description = "Description 2",
                    DateTime = DateTime.Now,
                    Category = "Music",
                    Location = "New York",
                    MaxUsers = 100,
                    ImageUrl = "imageUrl 2"
                },
            };
            context.Events.AddRange(events);
            await context.SaveChangesAsync();

            var repository = new EventRepository(context, _mapper);

            var (result, totalCount) = await repository.GetAllAsync(1, 5);

            Assert.Equal("Test Event", result.First().Name);
            Assert.Equal(2, totalCount);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectEvent()
        {
            await using var context = CreateContext();

            var eventId = Guid.NewGuid();
            var eventEntity = new EventEntity
            {
                Id = eventId,
                Name = "Test Event",
                Description = "Description 2",
                DateTime = DateTime.Now,
                Category = "Music",
                Location = "New York",
                MaxUsers = 100,
                ImageUrl = "imageUrl 2"
            };

            context.Events.Add(eventEntity);
            await context.SaveChangesAsync();

            var repository = new EventRepository(context, _mapper);

            var result = await repository.GetByIdAsync(eventId);

            Assert.NotNull(result);
            Assert.Equal("Test Event", result.Name);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEvent()
        {
            using var context = CreateContext();

            var eventToAdd = new Event(
                Guid.NewGuid(),
                "New Event",
                "Description 1",
                DateTime.Now,
                "Location 1",
                "Category 1",
                100
            );
            eventToAdd.AddImage("imageUrl 1");

            var repository = new EventRepository(context, _mapper);

            var addedEventId = await repository.AddAsync(eventToAdd);
            await context.SaveChangesAsync();
            var eventInDb = await context.Events.FindAsync(addedEventId);

            Assert.NotNull(eventInDb);
            Assert.Equal("New Event", eventInDb.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEvent()
        {
            await using var context = CreateContext();

            var eventId = Guid.NewGuid();
            var eventEntity = new EventEntity
            {
                Id = eventId,
                Name = "Event to Delete",
                Description = "Description",
                DateTime = DateTime.Now,
                Category = "Music",
                Location = "New York",
                MaxUsers = 100,
                ImageUrl = "ImageUrl"
            };

            context.Events.Add(eventEntity);
            await context.SaveChangesAsync();

            var repository = new EventRepository(context, _mapper);

            var deletedEventId = await repository.DeleteAsync(eventId);
            await context.SaveChangesAsync();
            var deletedEvent = await context.Events.FindAsync(deletedEventId);

            // Assert
            Assert.Null(deletedEvent);
        }


    }
}
