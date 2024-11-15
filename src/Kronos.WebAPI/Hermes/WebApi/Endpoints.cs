using System.Net.Mime;
using System.Security;
using System.Text;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.WebApi.Interop.Requests;
using Kronos.WebAPI.Hermes.WebApi.Interop.Responses;
using Kronos.WebAPI.Hermes.WebApi.Interop.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Hermes");
        var v1 = builder.MapGroup("/hermes/api/v{v:apiVersion}").HasApiVersion(1.0);
        v1.MapPost("/authenticate", PostAuthenticate)
            .Produces<AuthenticationSuccessfulResponse>(200, MediaTypeNames.Application.Json)
            .WithOpenApi(o =>
                new OpenApiOperation(o)
                {
                    Description = "Authenticate user",
                    OperationId = "authenticate"
                });

        v1.MapGet("/introspection", ([FromServices] PantheonRequestContext requestContext) => Task.FromResult(Results.Ok((object?)requestContext))).RequireAuthorization();

    }

    private static async Task<IResult> PostAuthenticate(
        [FromBody] AuthenticationPostRequest request,
        [FromServices] IHermesApi hermesApi,
        [FromServices] ILogger<AuthenticationPostRequest> logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenSet = await GetTokenSet(request, hermesApi, cancellationToken);

            return Results.Ok(new AuthenticationSuccessfulResponse(tokenSet.AccessToken, tokenSet.UserId,
                tokenSet.Scopes,
                tokenSet.Username));
        }
        catch (SecurityException e)
        {
            logger.LogError(e, "Login failed");
            return Results.Unauthorized();
        }
    }

    private static async Task<TokenSet> GetTokenSet(AuthenticationPostRequest request, IHermesApi hermesApi,
        CancellationToken cancellationToken)
    {
        return request.CredentialsType switch
        {
            CredentialsType.DeviceId => await hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!,
                request.RequestedScopes!.ToArray(),request.Audience, cancellationToken),
            CredentialsType.Password => await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                request.Password!, request.RequestedScopes!.ToArray(),request.Audience, cancellationToken),
            CredentialsType.Unknown => throw new ArgumentOutOfRangeException(),
            CredentialsType.AuthorizationCode => await hermesApi.CreateTokenSetForServiceAsync(request.ServiceId!.Value,
                Encoding.UTF8.GetBytes(request.AuthorizationCode!), request.RequestedScopes,request.Audience, cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}