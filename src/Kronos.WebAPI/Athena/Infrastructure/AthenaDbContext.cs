using Kronos.WebAPI.Athena.Data;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.Infrastructure;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountDataModel> UserAccounts { get; set; }
    public DbSet<ServiceAccountDataModel> ServiceAccounts { get; set; }
    public DbSet<ScopeDataModel> Scopes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");

        CreateUserAccounts(modelBuilder);
        CreateServiceAccounts(modelBuilder);

        modelBuilder.Entity<ScopeDataModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ScopeDataModel>()
            .Property(x => x.Description)
            .HasMaxLength(256);
        
        modelBuilder.Entity<ScopeDataModel>()
            .Property(x => x.DisplayName)
            .HasMaxLength(128);

        modelBuilder.Entity<ScopeDataModel>()
            .Property(x => x.Id)
            .HasMaxLength(128)
            .IsRequired();

        modelBuilder.Entity<UserAccountDataModel>()
            .HasMany(x => x.Scopes)
            .WithMany(x => x.UserAccounts)
            .UsingEntity<UserScopeDataModel>()
            .ToTable("UsersScopes");
    }

    private static void CreateServiceAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasKey(b => b.ServiceId);

        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.ServiceId);

        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.Secret)
            .HasMaxLength(256);
    }

    private static void CreateUserAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountDataModel>()
            .HasKey(b => b.UserId);

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.UserId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.DeviceId)
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.Username)
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.PasswordHash)
            .ValueGeneratedOnAdd();
    }
}