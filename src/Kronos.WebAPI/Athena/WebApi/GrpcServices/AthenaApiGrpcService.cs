using Athena.SDK;
using Grpc.Core;
using Kronos.WebAPI.Athena.Mappers;
using Kronos.WebAPI.Hermes.SDK;
using Pantheon.Athena.Grpc;

namespace Kronos.WebAPI.Athena.WebApi.GrpcServices;

public class AthenaApiGrpcService(IAthenaApi athenaApi, PantheonRequestContext requestContext) : AthenaApiGrpc.AthenaApiGrpcBase
{
    public override async Task<PantheonIdentity> CreateUserFromDeviceId(CreateUserFromDeviceIdRequest request,
        ServerCallContext context)
    {
        var identity = await athenaApi.CreateUserFromDeviceIdAsync(request.DeviceId, context.CancellationToken);
        return IdentityMappers.ToGrpc(identity);
    }

    public override async Task<VerifyPasswordResponse> VerifyPassword(VerifyPasswordRequest request,
        ServerCallContext context)
    {
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
        var doesExists = await athenaApi.DoesUsernameExistAsync(request.Username, context.CancellationToken);

        return new DoesUsernameExistResponse
        {
            DoesExist = doesExists
        };
    }

    public override async Task<PantheonIdentity> CreateUserFromUsername(CreateUserFromUsernameRequest request,
        ServerCallContext context)
    {
        var identity =
            await athenaApi.CreateUserFromUsernameAsync(request.Username, request.Password, context.CancellationToken);
        return IdentityMappers.ToGrpc(identity);
    }

    public override async Task<PantheonIdentity> GetUserByUsername(GetUserByUsernameRequest request,
        ServerCallContext context)
    {
        if (!requestContext.IsAuthenticated)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthenticated"));
        }

        var identity = await athenaApi.GetUserByUsernameAsync(request.Username, context.CancellationToken);
        
        if (identity is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"User with username {request.Username} not found"));
        
        if (identity.Id != requestContext.UserId!)
            throw new RpcException(new Status(StatusCode.NotFound, $"User with username {request.Username} not found"));
        
        return IdentityMappers.ToGrpc(identity);
    }

    public override async Task<PantheonIdentity> GetUserByDeviceId(GetUserByDeviceIdRequest request, ServerCallContext context)
    {
        var identity = await athenaApi.GetUserByDeviceIdAsync(request.DeviceId, context.CancellationToken);
        if (identity is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"User with deviceId {request.DeviceId} not found"));
        
        return IdentityMappers.ToGrpc(identity);
    }
}