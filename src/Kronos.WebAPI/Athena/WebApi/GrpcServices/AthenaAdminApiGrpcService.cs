using Athena.SDK;
using Grpc.Core;
using Kronos.WebAPI.Athena.Mappers;
using Pantheon.Athena.Grpc.Admin;
using Pantheon.Athena.Grpc.Common;

namespace Kronos.WebAPI.Athena.WebApi.GrpcServices;

public class AthenaAdminApiGrpcService(IAthenaAdminApi athenaAdminApi) : AthenaAdminApiGrpc.AthenaAdminApiGrpcBase
{
    public override async Task<PantheonIdentity> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var identity = await athenaAdminApi.CreateUserAsync(request.DeviceId, request.Username, request.Password, context.CancellationToken);
        return IdentityMappers.ToGrpc(identity);
    }
}