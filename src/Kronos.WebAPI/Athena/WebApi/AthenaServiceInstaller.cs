using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Athena.WebApi;

public static class AthenaServiceInstaller
{
    public static void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServiceDiscovery>().BindConfiguration("Athena:Discovery").ValidateDataAnnotations()
            .ValidateOnStart();
    }
}