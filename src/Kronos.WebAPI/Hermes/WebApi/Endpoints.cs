using System.Net.Mime;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Hermes");

        builder.MapGroup("/hermes/api/v{v:apiVersion}", api =>
        {
            api.HasApiVersion(1.0);
            api.MapPost("/authenticate", Handlers.AuthenticateAsync)
                .Produces<AuthenticationSuccessfulResponse>(200, MediaTypeNames.Application.Json)
                .WithOpenApi(o =>
                    new OpenApiOperation(o)
                    {
                        Description = "Authenticate user",
                        OperationId = "authenticate"
                    });

            api.MapGet("/introspection",
                    ([FromServices] PantheonRequestContext requestContext) => Results.Ok(requestContext))
                .RequireAuthorization();
        });

        builder.MapGroup("/hermes/api/admin/v{v:apiVersion}", api =>
        {
            api.HasApiVersion(1.0);
            api.MapGet("/audience/{id:guid}", Handlers.GetAudienceAsync)
                .Produces<AuthenticationSuccessfulResponse>(200, MediaTypeNames.Application.Json)
                .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);    
        });

        
    }
}