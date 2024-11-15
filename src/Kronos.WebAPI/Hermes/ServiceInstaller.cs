using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;
using Kronos.WebAPI.Hermes.WebApi;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Hermes;

public static class ServiceInstaller
{
    public static void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHermesApi, HermesApi>();
        services.AddOptions<HermesConfiguration>().BindConfiguration("HermesConfiguration").ValidateOnStart().ValidateDataAnnotations();
        services.AddOptions<JwtConfig>().BindConfiguration(GlobalDefinitions.ConfigurationKeys.HermesConfiguration).ValidateDataAnnotations().ValidateOnStart();
        services.AddPooledDbContextFactory<HermesDbContext>(opt => opt.UseNpgsql(
            configuration.GetConnectionString(GlobalDefinitions.ConfigurationKeys.PostgresConnectionString),
            x=>x.MigrationsHistoryTable("__EFMigrationsHistory", "hermes")));
        
    }
}