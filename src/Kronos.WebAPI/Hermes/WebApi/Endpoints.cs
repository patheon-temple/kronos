using System.Net.Mime;
using Kronos.Dtos;
using Kronos.WebAPI.Hermes.SDK;
using Microsoft.AspNetCore.Mvc;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Hermes");
        var v1 = builder.MapGroup("/hermes/api/v{v:apiVersion}").HasApiVersion(1.0);
        v1.MapPost("/authenticate", PostAuthenticate)
            .Produces<AuthenticationSuccessfulResponse>(200, MediaTypeNames.Application.Json)
            .WithDescription("Authentication endpoint");
    }

    private static async Task<IResult> PostAuthenticate(
        [FromBody] AuthenticationPostRequest request,
        [FromServices] IHermesApi hermesApi,
        CancellationToken cancellationToken = default)
    {
        var tokenSet = await GetTokenSet(request, hermesApi, cancellationToken);

        return Results.Ok(new AuthenticationSuccessfulResponse(tokenSet.AccessToken, tokenSet.UserId, tokenSet.Scopes,
            tokenSet.Username));
    }

    private static async Task<TokenSet> GetTokenSet(AuthenticationPostRequest request, IHermesApi hermesApi,
        CancellationToken cancellationToken)
    {
        return request.CredentialsType switch
        {
            CredentialsType.DeviceId => await hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!,
                request.RequestedScopes!.ToArray(), cancellationToken),
            CredentialsType.Password => await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                request.Password!, request.RequestedScopes!.ToArray(), cancellationToken),
            CredentialsType.Unknown => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}