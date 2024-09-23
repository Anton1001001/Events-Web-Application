using EventsWebApplication.Infrastructure.Configurations;
using EventsWebApplication.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EventsWebApplication.Infrastructure;

public sealed class EventsWebApplicationDbContext : DbContext
{
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<EventUserEntity> EventUsers { get; set; }
    
    
    public EventsWebApplicationDbContext(DbContextOptions<EventsWebApplicationDbContext> options) 
        : base(options)
    {
        Database.Migrate();
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}