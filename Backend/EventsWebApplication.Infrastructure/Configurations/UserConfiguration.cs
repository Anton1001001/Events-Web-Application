
using EventsWebApplication.Domain.Enums;
using EventsWebApplication.Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventsWebApplication.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName).HasMaxLength(100);
        builder.Property(p => p.LastName).HasMaxLength(100);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(320);


        builder.HasMany(r => r.RefreshTokens)
            .WithOne(r => r.UserEntity)
            .HasForeignKey(r => r.UserId);
        
        builder.HasData(new UserEntity
        {

            Id = new Guid("632d7af4-8c37-4235-ae45-b0dbf7451014"),
            Email = "admin@gmail.com",
            PasswordHash = "$2a$11$Je2fG7m05UbM0eEFR1W4oeV2i/w/mEdfM/FpfcDDXutgzGkdVnr7.",
            Role = Role.Admin
        });
    }

}