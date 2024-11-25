using Asp.Versioning.Builder;

namespace Kronos.WebAPI.Hermes.WebApi;

public static class RoutingExtensions
{
    public static RouteGroupBuilder MapGroup(this IVersionedEndpointRouteBuilder builder, string path,
        Action<RouteGroupBuilder> action)
    {
        var @group = builder.MapGroup(path);
        action(@group);
        return @group;
    }
}