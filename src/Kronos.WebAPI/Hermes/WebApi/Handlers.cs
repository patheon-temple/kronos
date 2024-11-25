using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Athena.WebApi.Interop.Responses;
using Kronos.WebAPI.Hermes.Extensions;
using Kronos.WebAPI.Hermes.WebApi.Interop.Requests;
using Kronos.WebAPI.Hermes.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class Handlers
{
    public static async Task<IResult> AuthenticateAsync(
        [FromBody] AuthenticationPostRequest request,
        [FromServices] IHermesApi hermesApi,
        [FromServices] IAthenaApi athenaApi,
        [FromServices] IHttpContextAccessor httpContext,
        [FromServices] ILogger<AuthenticationPostRequest> logger,
        CancellationToken cancellationToken = default)
    {
        var createTokenSetArgs = new CreateTokenSetArgs
        {
            CredentialsType = request.CredentialsType,
            Username = request.Username,
            AuthorizationCode = request.AuthorizationCode,
            Password = request.Password,
            DeviceId = request.DeviceId,
            Audience = request.Audience,
            ServiceId = request.ServiceId,
            RequestedScopes = request.RequestedScopes
        };

        var (result, error) = await hermesApi.CreateTokenSetAsync(createTokenSetArgs, cancellationToken);
        
        if (error is null)
        {
            if (httpContext.HasRequestHeaderValue(GlobalDefinitions.Headers.ValidateOnly,
                    "true")) return Results.NoContent();

            return Results.Ok(new AuthenticationSuccessfulResponse(result!.AccessToken,
                result.UserId, result.Scopes, result.Username));
        }

        return error switch
        {
            CreateTokenSetError.NonExistingAudience => Results.Problem(statusCode: StatusCodes.Status401Unauthorized,
                title: "Audience doesn't exists", detail: error.ToString()),
            CreateTokenSetError.InvalidCredentials => Results.Problem(statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials", detail: error.ToString()),
            CreateTokenSetError.ServiceCredentialsInvalid => Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized, title: "Invalid credentials", detail: error.ToString()),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    public static async Task<IResult> GetAudienceAsync([FromRoute(Name = "id")] Guid id,
        [FromServices] IHermesAdminApi hermesAdminApi,
        [FromServices] IAthenaAdminApi athenaAdminApi,
        CancellationToken cancellationToken = default)
    {
        var (data, error) = await hermesAdminApi.GetOrCreateTokenCryptoDataAsync(id, cancellationToken);
        
        if (error is null)
            return Results.Ok(new GetAudienceCryptoResponse
            {
                Expiration = data!.Expiration,
                EncryptionKey = Convert.ToBase64String(data.EncryptionKey),
                SigningKey = Convert.ToBase64String(data.SigningKey)
            });
        
        return error switch
        {
            GetOrCreateTokenCryptoDataError.ServiceAccountNotFound => Results.Problem(
                statusCode: StatusCodes.Status404NotFound, title: "Audience doesn't exists"),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}