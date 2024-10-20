using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.Infrastructure;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<PantheonIdentityDataModel> PantheonIdentities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");
        modelBuilder.Entity<PantheonIdentityDataModel>()
            .HasKey(b => b.Id)
            .HasName("PK_pantheon_identities");

        modelBuilder.Entity<PantheonIdentityDataModel>()
            .ToTable("pantheon_identities");
        
        modelBuilder.Entity<PantheonIdentityDataModel>()
            .Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<PantheonIdentityDataModel>()
            .Property(b => b.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();
    }
}