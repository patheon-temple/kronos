using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.Infrastructure;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountDataModel> PantheonIdentities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");

        modelBuilder.Entity<UserAccountDataModel>()
            .ToTable("user_accounts");

        modelBuilder.Entity<UserAccountDataModel>()
            .HasKey(b => b.UserId)
            .HasName("PK_user_account_id");

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.UserId)
            .HasColumnName("user_id")
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.DeviceId)
            .HasColumnName("device_id")
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();
        
        
        modelBuilder.Entity<ServiceAccountDataModel>()
            .ToTable("service_accounts");

        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasKey(b => b.ServiceId)
            .HasName("PK_service_account_id");

        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.ServiceId)
            .HasColumnName("service_id");

        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.Secret)
            .HasColumnName("secret")
            .HasMaxLength(128);
    }
}