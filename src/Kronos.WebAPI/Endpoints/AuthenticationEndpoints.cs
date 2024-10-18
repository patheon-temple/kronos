using Microsoft.AspNetCore.Mvc;
using Zeus.Interop.Requests;
using Zeus.Interop.Responses;
using Zeus.Services;

namespace Zeus.Endpoints;

public static class AuthenticationEndpoints
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