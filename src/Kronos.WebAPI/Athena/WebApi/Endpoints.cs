namespace Kronos.WebAPI.Athena.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var apiV1 = builder.MapGroup("/athena/api/v{version:apiVersion}").HasApiVersion(1.0);

        apiV1
            .MapGet("/identities/me", Identities.GetMe)
            .MapToApiVersion(1.0)
            .RequireAuthorization();
    }
}

internal static class Identities
{
    public static IResult GetMe()
    {
        return Results.Empty;
    }
}