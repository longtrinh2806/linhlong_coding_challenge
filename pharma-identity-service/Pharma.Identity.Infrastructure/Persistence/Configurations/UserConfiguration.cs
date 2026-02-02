using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharma.Identity.Application;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user", Constant.IdentityDatabaseSchema);

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasConversion(
                convertToProviderExpression: userId => userId.ToString(),
                convertFromProviderExpression: userId => Ulid.Parse(userId)
            )
            .HasMaxLength(26);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
    }
}