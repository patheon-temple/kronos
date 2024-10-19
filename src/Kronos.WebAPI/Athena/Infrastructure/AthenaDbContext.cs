using Kronos.WebAPI.Athena.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.Infrastructure;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<PantheonIdentity> PantheonIdentities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");
        modelBuilder.Entity<PantheonIdentity>()
            .HasKey(b => b.Id)
            .HasName("PK_pantheon_identities");
        
        modelBuilder.Entity<PantheonIdentity>()
            .ToTable("pantheon_identities")
            .Property(b => b.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
    }
}