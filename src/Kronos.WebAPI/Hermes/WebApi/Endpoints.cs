using System.Net.Mime;
using Athena.SDK;
using Hermes.SDK;
using Kronos.WebAPI.Athena.WebApi.Interop.Responses;
using Kronos.WebAPI.Hermes.Extensions;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.WebApi.Interop.Requests;
using Kronos.WebAPI.Hermes.WebApi.Interop.Responses;
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
                var result = await hermesAdminApi.GetOrCreateTokenCryptoDataAsync(id, cancellationToken);
                return result.Status switch
                {
                    GetOrCreateTokenCryptoDataResult.Success => Results.Ok(new GetAudienceCryptoResponse
                    {
                        Expiration = result.Data!.Expiration,
                        EncryptionKey = Convert.ToBase64String(result.Data.EncryptionKey),
                        SigningKey = Convert.ToBase64String(result.Data.SigningKey)
                    }),
                    GetOrCreateTokenCryptoDataResult.Failure => Results.StatusCode(StatusCodes
                        .Status500InternalServerError),
                    _ => throw new ArgumentOutOfRangeException()
                };
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


        var result = await hermesApi.CreateTokenSetAsync(createTokenSetArgs, cancellationToken);
        return result.Status switch
        {
            CreateTokenSetResult.Success when httpContext.HasRequestHeaderValue(GlobalDefinitions.Headers.ValidateOnly,
                "true") => Results.NoContent(),
            CreateTokenSetResult.Success => Results.Ok(new AuthenticationSuccessfulResponse(result.Data.AccessToken,
                result.Data.UserId, result.Data.Scopes, result.Data.Username)),
            CreateTokenSetResult.Failure => Results.StatusCode(StatusCodes.Status500InternalServerError),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}