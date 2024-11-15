using Kronos.WebAPI.Hermes.SDK;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Hermes.Services;

public class HermesDbContext(DbContextOptions<HermesDbContext> options) : DbContext(options)
{
    public DbSet<TokenCryptoDataModel> TokenCryptoData { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("hermes");

        modelBuilder.Entity<TokenCryptoDataModel>().HasKey(x => x.EntityId);
        modelBuilder.Entity<TokenCryptoDataModel>().Property(x => x.EncryptionKey)
            .HasMaxLength(GlobalDefinitions.Limits.EncryptionKeyMaxSize).IsRequired();
        modelBuilder.Entity<TokenCryptoDataModel>().Property(x => x.SigningKey)
            .HasMaxLength(GlobalDefinitions.Limits.EncryptionKeyMaxSize).IsRequired();
    }
}