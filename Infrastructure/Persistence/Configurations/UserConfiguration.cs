using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.NationalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.NationalCode)
            .IsUnique();

        builder.Property(x => x.Version)
            .IsConcurrencyToken();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}