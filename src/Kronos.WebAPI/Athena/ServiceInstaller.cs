using Athena.SDK;
using FluentValidation;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.SDK;
using Kronos.WebAPI.Athena.WebApi.HostedServices;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena;

public static class ServiceInstaller
{
    public static void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<SuperuserHostedService>();
        services.AddOptions<AthenaConfiguration>().BindConfiguration("AthenaConfiguration").ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IAthenaApi, AthenaApi>();
        services.AddPooledDbContextFactory<AthenaDbContext>(opt => 
            opt.UseNpgsql(configuration.GetConnectionString("Postge_Athena")));
        services.AddGrpc();
        services.AddTransient<IValidator<UserCredentialsValidationParams>>(x => Passwords.CreateValidator());
    }
}