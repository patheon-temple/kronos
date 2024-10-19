using Zeus.Services;

namespace Kronos.WebAPI.Zeus;

public static class ZeusServiceInstaller
{
    public static void Install(IServiceCollection services)
    {
        
        services.AddScoped<TokenService>();
    }
}