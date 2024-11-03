using Grpc.Core;
using Kronos.WebAPI.Hermes.SDK;

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