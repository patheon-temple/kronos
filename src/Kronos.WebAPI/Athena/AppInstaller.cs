using Kronos.WebAPI.Athena.WebApi.GrpcServices;

namespace Kronos.WebAPI.Athena;

public static class AppInstaller
{
    public static void Install(IEndpointRouteBuilder app)
    {
        app.MapGrpcService<AthenaApiGrpcService>();
        app.MapGrpcService<AthenaAdminApiGrpcService>();
    }
}
