using System.Net.Mime;
using System.Security;
using System.Text;
using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Athena.WebApi.Interop.Responses;
using Kronos.WebAPI.Hermes.Mappers;
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

        v1.MapGet("/introspection",
            ([FromServices] PantheonRequestContext requestContext) =>
                Task.FromResult(Results.Ok((object?)requestContext))).RequireAuthorization();

        var adminV1 = builder.MapGroup("/hermes/api/admin/v{v:apiVersion}").HasApiVersion(1.0);

        adminV1.MapGet("/audience/{id:guid}", async ([FromRoute(Name = "id")] Guid id,
                [FromServices] IHermesAdminApi hermesAdminApi,
                [FromServices] IAthenaAdminApi athenaAdminApi,
                CancellationToken cancellationToken = default) =>
            {
                if (!await athenaAdminApi.ServiceAccountExistsAsync(id, cancellationToken))
                {
                    return Results.Problem($"Audience with id {id} doesn't exist",
                        statusCode: StatusCodes.Status424FailedDependency, title: "Audience doesn't exist");
                }

                var data = await hermesAdminApi.GetOrCreateTokenCryptoDataAsync(id, cancellationToken);
                return Results.Ok(new GetAudienceCryptoResponse
                {
                    Expiration = data.Expiration,
                    EncryptionKey = Convert.ToBase64String(data.EncryptionKey),
                    SigningKey = Convert.ToBase64String(data.SigningKey)
                });
            }
        ).RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
    }

    private static async Task<IResult> PostAuthenticate(
        [FromBody] AuthenticationPostRequest request,
        [FromServices] IHermesApi hermesApi,
        [FromServices] IAthenaApi athenaApi,
        [FromServices] IHttpContextAccessor httpContext,
        [FromServices] ILogger<AuthenticationPostRequest> logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (httpContext.HttpContext != null &&
                httpContext.HttpContext.Request.Headers.TryGetValue(GlobalDefinitions.Headers.ValidateOnly,
                    out var value))
            {
                if (value.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    var isValid = await ValidateCredentials(request, athenaApi, cancellationToken);
                    return isValid ? Results.NoContent() : Results.Unauthorized();
                }
            }

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

    private static async Task<bool> ValidateCredentials(AuthenticationPostRequest request, IAthenaApi athenaApi,
        CancellationToken cancellationToken)
    {
        switch (request.CredentialsType)
        {
            case CredentialsType.Password:
                return await athenaApi.ValidateUserCredentialsByUsernameAsync(request.Username!, request.Password!,
                    cancellationToken);
            case CredentialsType.AuthorizationCode:
                return await athenaApi.ValidateServiceCodeAsync(request.ServiceId!.Value, request.AuthorizationCode!,
                    cancellationToken);
            case CredentialsType.DeviceId:
            case CredentialsType.Unknown:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task<TokenSet> GetTokenSet(AuthenticationPostRequest request, IHermesApi hermesApi,
        CancellationToken cancellationToken)
    {
        return request.CredentialsType switch
        {
            CredentialsType.DeviceId => await hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!,
                request.Password!, request.RequestedScopes!.ToArray(), request.Audience, cancellationToken),
            CredentialsType.Password => await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                request.Password!, request.RequestedScopes!.ToArray(), request.Audience, cancellationToken),
            CredentialsType.AuthorizationCode => await hermesApi.CreateTokenSetForServiceAsync(request.ServiceId!.Value,
                request.AuthorizationCode!, request.RequestedScopes, request.Audience,
                cancellationToken),
            CredentialsType.Unknown => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}