using Athena.SDK;
using Grpc.Core;
using Kronos.WebAPI.Athena.Mappers;
using Kronos.WebAPI.Hermes.SDK;
using Pantheon.Athena.Grpc;
using Pantheon.Athena.Grpc.Common;

namespace Kronos.WebAPI.Athena.WebApi.GrpcServices;

public static class PantheonRequestContextExtensions
{
    public static PantheonRequestContext AuthenticatedOrThrow(this PantheonRequestContext requestContext)
    {
        if (!requestContext.IsAuthenticated)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthenticated"));
        }

        return requestContext;
    }

    public static PantheonRequestContext AuthorizedOrThrow(this PantheonRequestContext requestContext, params string[]? scopes)
    {
        if (scopes == null || scopes.Length == 0) return requestContext;

        var missingScopes = scopes.Where(s => !requestContext.Scopes.Contains(s)).ToArray();
        if (missingScopes.Length <= 0) return requestContext;

        throw new RpcException(new Status(StatusCode.PermissionDenied,
            $"Missing scopes: {string.Join(",", missingScopes)}"));
    }
}

public class AthenaApiGrpcService(IAthenaApi athenaApi, PantheonRequestContext requestContext)
    : AthenaApiGrpc.AthenaApiGrpcBase
{
    public override async Task<PantheonIdentity> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var identity =
            await athenaApi.GetUserByIdAsync(new Guid(request.UserId.ToByteArray()), context.CancellationToken);
        return ProcessRequestInternally(identity);
    }

    public override async Task<PantheonIdentity> CreateUserFromDeviceId(CreateUserFromDeviceIdRequest request,
        ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var identity = await athenaApi.CreateUserFromDeviceIdAsync(request.DeviceId, context.CancellationToken);
        return IdentityMappers.ToGrpc(identity);
    }


    public override async Task<VerifyPasswordResponse> VerifyPassword(VerifyPasswordRequest request,
        ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var isValid =
            await athenaApi.VerifyPasswordAsync(new Guid(request.UserId.ToByteArray()), request.Password,
                context.CancellationToken);
        return new VerifyPasswordResponse
        {
            IsValid = isValid
        };
    }

    public override async Task<DoesUsernameExistResponse> DoesUsernameExist(DoesUsernameExistRequest request,
        ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var doesExists = await athenaApi.DoesUsernameExistAsync(request.Username, context.CancellationToken);

        return new DoesUsernameExistResponse
        {
            DoesExist = doesExists
        };
    }

    public override async Task<PantheonIdentity> CreateUserFromUsername(CreateUserFromUsernameRequest request,
        ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var identity =
            await athenaApi.CreateUserFromUsernameAsync(request.Username, request.Password, context.CancellationToken);
        return IdentityMappers.ToGrpc(identity);
    }

    public override async Task<PantheonIdentity> GetUserByUsername(GetUserByUsernameRequest request,
        ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var identity = await athenaApi.GetUserByUsernameAsync(request.Username, context.CancellationToken);
        return ProcessRequestInternally(identity);
    }

    public override async Task<PantheonIdentity> GetUserByDeviceId(GetUserByDeviceIdRequest request,
        ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow();
        var identity = await athenaApi.GetUserByDeviceIdAsync(request.DeviceId, context.CancellationToken);
        return ProcessRequestInternally(identity);
    }


    private PantheonIdentity ProcessRequestInternally(global::Athena.SDK.Models.PantheonIdentity? identity)
    {
        if (identity is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"User  not found"));

        if (identity.Id != requestContext.UserId!)
            throw new RpcException(new Status(StatusCode.Unauthenticated,
                $"You're not authorized to access this user"));

        return IdentityMappers.ToGrpc(identity);
    }
}