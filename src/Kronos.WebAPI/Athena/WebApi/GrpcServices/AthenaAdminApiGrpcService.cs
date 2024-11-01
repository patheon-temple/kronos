using System.Collections.Immutable;
using Athena.SDK;
using Grpc.Core;
using Kronos.WebAPI.Athena.Mappers;
using Kronos.WebAPI.Hermes.SDK;
using Pantheon.Athena.Grpc.Admin;
using Pantheon.Athena.Grpc.Common;

namespace Kronos.WebAPI.Athena.WebApi.GrpcServices;

public class AthenaAdminApiGrpcService(IAthenaAdminApi athenaAdminApi, PantheonRequestContext requestContext)
    : AthenaAdminApiGrpc.AthenaAdminApiGrpcBase
{
    public override async Task<PantheonIdentity> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        requestContext.AuthenticatedOrThrow().AuthorizedOrThrow(WebAPI.Definitions.Scopes.Superuser);

        var identity = await athenaAdminApi.CreateUserAsync(request.DeviceId, request.Username, request.Password,
            request.DefaultScopes.ToArray(), context.CancellationToken);
        return IdentityMappers.ToGrpc(identity);
    }
}