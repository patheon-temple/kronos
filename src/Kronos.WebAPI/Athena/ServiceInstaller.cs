using Athena.SDK;
using FluentValidation;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.SDK;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena;

public static class ServiceInstaller
{
    public static void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AthenaConfiguration>().BindConfiguration("AthenaConfiguration").ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IAthenaApi, AthenaApi>();
        services.AddScoped<IAthenaAdminApi, AthenaAdminApi>();
        services.AddPooledDbContextFactory<AthenaDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString(GlobalDefinitions.ConfigurationKeys.PostgresConnectionString),
                x=>x.MigrationsHistoryTable("__EFMigrationsHistory", "athena")));
        services.AddTransient<IValidator<UserCredentialsValidationParams>>(x => Passwords.CreateValidator());
    }
}