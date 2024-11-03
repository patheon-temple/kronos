using Kronos.WebAPI.Hermes.WebApi.GrpcServices;

namespace Kronos.WebAPI.Hermes;

public static class AppInstaller
{
    public static void Install(IEndpointRouteBuilder app)
    {
        app.MapGrpcService<HermesGrpcApi>();
    }   
}