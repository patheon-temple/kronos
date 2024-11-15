using Kronos.WebAPI.Athena.Data;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.Infrastructure;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountDataModel> UserAccounts { get; set; }
    public DbSet<ServiceAccountDataModel> ServiceAccounts { get; set; }
    public DbSet<UserScopeDataModel> UserScopes { get; set; }
    public DbSet<ServiceScopeDataModel> ServiceScopes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");

        CreateUserAccounts(modelBuilder);
        CreateServiceAccounts(modelBuilder);
        CreateScopes(modelBuilder);
        CreateUserScopes(modelBuilder);
        CreateServiceScopes(modelBuilder);
    }

    private static void CreateScopes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserScopeDataModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<UserScopeDataModel>()
            .Property(x => x.Description)
            .HasMaxLength(256);

        modelBuilder.Entity<UserScopeDataModel>()
            .Property(x => x.DisplayName)
            .HasMaxLength(128);

        modelBuilder.Entity<UserScopeDataModel>()
            .Property(x => x.Id)
            .HasMaxLength(128)
            .IsRequired();
    }

    private static void CreateServiceScopes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasMany(x => x.Scopes)
            .WithMany(x => x.Accounts)
            .UsingEntity<ServiceAccountScopeDataModel>()
            .ToTable("ServicesScopes");
    }

    private static void CreateUserScopes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountDataModel>()
            .HasMany(x => x.Scopes)
            .WithMany(x => x.Accounts)
            .UsingEntity<UserAccountScopeDataModel>()
            .ToTable("UsersScopes");
    }

    private static void CreateServiceAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceAccountDataModel>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ServiceAccountDataModel>().Property(b=>b.Name).HasMaxLength(256)
            .IsRequired();
        
        modelBuilder.Entity<ServiceAccountDataModel>()
            .Property(b => b.AuthorizationCode)
            .HasMaxLength(256);
    }

    private static void CreateUserAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccountDataModel>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.Id)
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
            .Property(b => b.PasswordHash);
    }
}