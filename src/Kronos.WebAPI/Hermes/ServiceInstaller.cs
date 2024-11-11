using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;
using Kronos.WebAPI.Hermes.WebApi;

namespace Kronos.WebAPI.Hermes;

public static class ServiceInstaller
{
    public static void Install(IServiceCollection services)
    {
        services.AddScoped<IHermesApi, HermesApi>();
        services.AddScoped<TokenService>();
        services.AddOptions<HermesConfiguration>().BindConfiguration("HermesConfiguration").ValidateOnStart().ValidateDataAnnotations();
        // TODO: merge with HermesConfiguration
        services.AddOptions<JwtConfig>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart();
        
    }
}