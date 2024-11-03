using System.Security;
using FluentValidation;
using Grpc.Core;
using Kronos.WebAPI.Hermes.SDK;
using Pantheon.Hermes.Grpc;

namespace Kronos.WebAPI.Hermes.WebApi.GrpcServices;

public class HermesGrpcApi(IHermesApi hermesApi, ILogger<HermesGrpcApi> logger) : HermesApiGrpc.HermesApiGrpcBase
{
    public override async Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
    {
        return request.CredentialsType switch
        {
            CredentialsType.DeviceId => await DeviceIdTokenSet(request, context.CancellationToken),
            CredentialsType.Password => await UserCredentialsTokenSet(request, context.CancellationToken),
            CredentialsType.Unknown => throw new RpcException(new Status(StatusCode.Unauthenticated,
                $"Invalid CredentialsType: {request.CredentialsType}")),
            _ => throw new RpcException(new Status(StatusCode.Unauthenticated,
                $"Invalid CredentialsType: {request.CredentialsType}"))
        };
    }


    private async Task<GetTokenResponse> UserCredentialsTokenSet(GetTokenRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tokenSet = await hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                request.Password!, request.RequestedScopes.ToArray(), cancellationToken);

            return new GetTokenResponse
            {
                AccessToken = tokenSet.AccessToken,
                Id = tokenSet.UserId,
                Scopes = { tokenSet.Scopes },
                Username = tokenSet.Username ?? string.Empty
            };
        }
        catch (SecurityException e)
        {
            logger.LogWarning(e, "Security exception");
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Invalid credentials"));
        }
        catch (ValidationException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
    }

    private async Task<GetTokenResponse> DeviceIdTokenSet(GetTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var tokenSet =
            await hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!, request.RequestedScopes.ToArray(),
                cancellationToken);
        return new GetTokenResponse
        {
            AccessToken = tokenSet.AccessToken,
            Id = tokenSet.UserId,
            Scopes = { tokenSet.Scopes },
            Username = tokenSet.Username ?? string.Empty
        };
    }
}