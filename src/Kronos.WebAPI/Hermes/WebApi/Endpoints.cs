using System.Security;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.WebApi.Interop.Requests;
using Kronos.WebAPI.Hermes.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Hermes");
        var apiV1 = builder.MapGroup("/hermes/api/v{version:apiVersion}").HasApiVersion(1.0);

        apiV1
            .MapPost("/tokens", Tokens.Post)
            .Accepts<AuthenticationPostRequest>("application/json")
            .Produces<AuthenticationSuccessfulResponse>()
            .Produces(400)
            .ProducesValidationProblem()
            .MapToApiVersion(1.0);
    }

    private static class Tokens
    {
        public static async Task<IResult> Post(
            [FromBody] AuthenticationPostRequest request,
            [FromServices] IHermesApi hermesApi,
            [FromServices] ILogger<AuthenticationPostRequest> logger,
            CancellationToken cancellationToken = default)
        {
            var validation = await Passwords.CreateValidator().ValidateAsync(new
                UserCredentialsValidationParams
                {
                    Password = request.Password,
                    Username = request.Username,
                }, cancellationToken);
            
            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }

            return request.CredentialsType switch
            {
                CredentialsType.DeviceId => await DeviceIdTokenSet(request, hermesApi, cancellationToken),
                CredentialsType.Password => await UserCredentialsTokenSet(request, hermesApi, logger, cancellationToken),
                CredentialsType.Unknown => Results.Problem($"Unknown credentials type: {request.CredentialsType}",
                    statusCode: StatusCodes.Status501NotImplemented),
                _ => Results.Problem($"Unknown credentials type: {request.CredentialsType}",
                    statusCode: StatusCodes.Status501NotImplemented)
            };
        }

        private static async Task<IResult> UserCredentialsTokenSet(AuthenticationPostRequest request, IHermesApi hermesApi, ILogger<AuthenticationPostRequest> logger, CancellationToken cancellationToken)
        {
            try
            {
                var tokenSet = await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                    request.Password!, cancellationToken);
                
                return Results.Ok(new AuthenticationSuccessfulResponse
                {
                    AccessToken = tokenSet.AccessToken
                });
            }
            catch (SecurityException e)
            {
                logger.LogWarning(e, "Security exception");
                return Results.Unauthorized();
            }
        }

        private static async Task<IResult> DeviceIdTokenSet(AuthenticationPostRequest request, IHermesApi hermesApi, CancellationToken cancellationToken = default)
        {
            var tokenSet = await hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!, cancellationToken);
            return Results.Ok(new AuthenticationSuccessfulResponse
            {
                AccessToken = tokenSet.AccessToken
            });
        }
    }
}