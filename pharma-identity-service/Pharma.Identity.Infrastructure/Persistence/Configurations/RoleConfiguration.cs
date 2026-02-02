using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pharma.Identity.Application;
using Pharma.Identity.Domain.Entities;

namespace Pharma.Identity.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("role", Constant.IdentityDatabaseSchema);

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasData(
            new Role { RoleId = 1, Name = "Editor" },
            new Role { RoleId = 2, Name = "Viewer" }
        );
    }
}