using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pharma.Identity.Domain.Entities;
using Pharma.Identity.Infrastructure.Persistence.Configurations;

namespace Pharma.Identity.Infrastructure.Persistence;

public class ReadOnlyDbContext(DbContextOptions<ReadOnlyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());

        var ulidConverter = new ValueConverter<Ulid, string>(
            v => v.ToString(),
            v => Ulid.Parse(v)
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var ulidProps = entityType.ClrType
                .GetProperties()
                .Where(p => p.PropertyType == typeof(Ulid));

            foreach (var prop in ulidProps)
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Property(prop.Name)
                    .HasConversion(ulidConverter)
                    .HasMaxLength(26)
                    .HasColumnType("varchar(26)");
            }
        }
    }
}