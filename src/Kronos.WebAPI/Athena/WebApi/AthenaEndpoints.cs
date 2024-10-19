using System.Net.Mime;
using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Athena.WebApi;

public static class AthenaEndpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var v1 = builder.MapGroup("/athena/api/v{v:apiVersion}").HasApiVersion(1.0);
        v1.MapGet("/", Discovery.Get)
            .Produces<ServiceDiscoveryResponse>(200, MediaTypeNames.Application.Json)
            .WithDescription("Discovery endpoint for rest of the serices");
    }

    private static class Discovery
    {
        public static IResult Get([FromServices] IOptions<ServiceDiscovery> options)
        {
            return Results.Ok(new ServiceDiscoveryResponse
            {
                Zeus = new ServiceDescriptionResponse
                {
                    Url = options.Value.Zeus.Url
                }
            });
        }
    }
}