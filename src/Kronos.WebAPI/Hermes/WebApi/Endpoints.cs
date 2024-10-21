using Kronos.WebAPI.Athena.SDK;
using Kronos.WebAPI.Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;
using Kronos.WebAPI.Hermes.WebApi.Interop.Requests;
using Kronos.WebAPI.Hermes.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Hermes");
        var tokensV1 = builder.MapGroup("/hermes/api").HasApiVersion(1.0);

        tokensV1
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
            [FromServices] IAthenaApi athenaApi,
            CancellationToken cancellationToken = default)
        {
            switch (request.CredentialsType)
            {
                case CredentialsType.DeviceId:
                    if (string.IsNullOrWhiteSpace(request.DeviceId))
                        return Results.ValidationProblem(new Dictionary<string, string[]>
                        {
                            {
                                nameof(request.DeviceId),
                                [$"Credentials type {CredentialsType.DeviceId} require  {nameof(request.DeviceId)}"]
                            }
                        });

                    var identity = await athenaApi.GetIdentityByDeviceIdAsync(request.DeviceId, cancellationToken);
                    
                    if (identity is null) return Results.Unauthorized();
                    
                    var tokenSet = hermesApi.CreateTokenSetForIdentity(identity);

                    return Results.Ok(new AuthenticationSuccessfulResponse
                    {
                        AccessToken = tokenSet.AccessToken
                    });
                case CredentialsType.Unknown:
                default:
                {
                    return Results.Problem($"Unknown credentials type: {request.CredentialsType}",
                        statusCode: StatusCodes.Status501NotImplemented);
                }
            }
        }
    }
}