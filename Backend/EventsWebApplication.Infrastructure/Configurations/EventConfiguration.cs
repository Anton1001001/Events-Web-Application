using EventsWebApplication.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsWebApplication.Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.DateTime).IsRequired();
        builder.Property(e => e.Location).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Category).IsRequired().HasMaxLength(150);
        builder.Property(e => e.MaxUsers).IsRequired();
        builder.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
        builder.HasMany<UserEntity>(p => p.Users)
            .WithMany(e => e.Events)
            .UsingEntity<EventUserEntity>(
                j => j
                    .HasOne<UserEntity>()
                    .WithMany()
                    .HasForeignKey(u => u.UserId),
                j => j
                    .HasOne<EventEntity>()
                    .WithMany()
                    .HasForeignKey(e => e.EventId),
                j =>
                {
                    j.HasKey(k => new {k.UserId, k.EventId});
                    j.Property(eu => eu.RegistrationDate).IsRequired().ValueGeneratedOnAdd(); 
                }
            );
        

    }
}