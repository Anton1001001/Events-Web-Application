using EventsWebApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsWebApplication.Infrastructure.Configurations;

public class EventUserConfiguration : IEntityTypeConfiguration<EventUser>
{
    public void Configure(EntityTypeBuilder<EventUser> builder)
    {
        builder.ToTable("EventUsers");
        
        builder.HasKey(eu => new { eu.UserId, eu.EventId });

        builder
            .HasOne(eu => eu.User)
            .WithMany(u => u.EventUsers)
            .HasForeignKey(eu => eu.UserId);
        
        builder
            .HasOne(eu => eu.Event)
            .WithMany(e => e.EventUsers)
            .HasForeignKey(eu => eu.EventId);
    }
}