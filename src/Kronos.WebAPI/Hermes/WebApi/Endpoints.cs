using System.Text;
using FluentValidation;
using Kronos.WebAPI.Athena.Crypto;
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

            switch (request.CredentialsType)
            {
                case CredentialsType.DeviceId:
                {
                    var tokenSet = await hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!, cancellationToken);
                    return Results.Ok(new AuthenticationSuccessfulResponse
                    {
                        AccessToken = tokenSet.AccessToken
                    });
                }
                case CredentialsType.Password:
                {
                    var tokenSet = await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                        request.Password!, cancellationToken);
                    return Results.Ok(new AuthenticationSuccessfulResponse
                    {
                        AccessToken = tokenSet.AccessToken
                    });
                }
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