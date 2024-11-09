using System.Net.Mime;
using Kronos.WebAPI.Kronos.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Kronos.WebAPI.Kronos.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Kronos").WithTags("Kronos");
        var v1 = builder.MapGroup("/kronos/api/v{v:apiVersion}").HasApiVersion(1.0);
        v1.MapGet("/", ([FromServices] IOptionsSnapshot<ServiceDiscovery> options) => Results.Ok(options.Value))
            .WithOpenApi(o =>
            new OpenApiOperation(o)
            {
                Description = "Services discovery endpoint",
                OperationId = "discovery"
            })
            .Produces<ServiceDiscovery>(200, MediaTypeNames.Application.Json);
    }

    private static class Discovery
    {
        
        public static IResult Get([FromServices] IOptionsSnapshot<ServiceDiscovery> options) => Results.Ok(options.Value);
    }
}