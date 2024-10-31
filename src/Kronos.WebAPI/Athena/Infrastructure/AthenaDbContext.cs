using Kronos.WebAPI.Athena.Data;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena.Infrastructure;

public sealed class AthenaDbContext(DbContextOptions<AthenaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccountDataModel> UserAccounts { get; set; }
    public DbSet<ServiceAccountDataModel> ServiceAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("athena");

        CreateUserAccounts(modelBuilder);
        CreateServiceAccounts(modelBuilder);

        modelBuilder.Entity<ScopeDataModel>().ToTable("scopes");

        modelBuilder.Entity<ScopeDataModel>()
            .HasKey(x => x.Id)
            .HasName("PK_scope_id");

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
            .ToTable("users_scopes");
    }

    private static void CreateServiceAccounts(ModelBuilder modelBuilder)
    {
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
            .HasMaxLength(256);
    }

    private static void CreateUserAccounts(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.Username)
            .HasColumnName("username")
            .HasMaxLength(128)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<UserAccountDataModel>()
            .Property(b => b.PasswordHash)
            .HasColumnName("password_hash")
            .ValueGeneratedOnAdd();
    }
}