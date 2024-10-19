using Kronos.WebAPI.Athena.Infrastructure;
using Kronos.WebAPI.Athena.Services;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI.Athena;

public static class ServiceInstaller
{
    public static void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IIdentityRepository, PgsqlIdentityRepository>();
        services.AddDbContextPool<AthenaDbContext>(opt => 
            opt.UseNpgsql(configuration.GetConnectionString("Postge_Athena")));
    }
}