using System.Net.Mime;
using System.Security;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
                request.RequestedScopes!.ToArray(), cancellationToken),
            CredentialsType.Password => await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                request.Password!, request.RequestedScopes!.ToArray(), cancellationToken),
            CredentialsType.Unknown => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public record AuthenticationSuccessfulResponse(string AccessToken, string UserId, string[] Scopes, string? Username);

public class AuthenticationPostRequest
{
    public AuthenticationPostRequest(string? username)
    {
        Username = username;
    }

    public CredentialsType CredentialsType { get; set; } = CredentialsType.DeviceId;
    public string[] RequestedScopes { get; set; } = [];
    public string? Password { get; set; }
    public string? DeviceId { get; set; }
    public string? Username { get; set; }
}

public enum CredentialsType
{
    Unknown = 0,
    DeviceId = 1,
    Password
}