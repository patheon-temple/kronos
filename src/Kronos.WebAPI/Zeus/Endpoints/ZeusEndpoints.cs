using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Zeus.Interop.Requests;
using Zeus.Interop.Responses;
using Zeus.Services;

namespace Kronos.WebAPI.Zeus.Endpoints;

public static class ZeusEndpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Zeus");
        var tokensV1 = builder.MapGroup("/zeus/api").HasApiVersion(1.0);
       
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
        public static IResult Post(
            [FromBody] AuthenticationPostRequest request,
            [FromServices] TokenService tokenService,
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
                
                    var accessToken = tokenService.CreateAccessToken(new TokenCreationArgs
                    {
                        DeviceId = request.DeviceId,
                    });
                
                    return Results.Ok(new AuthenticationSuccessfulResponse
                    {
                        AccessToken = accessToken
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