﻿namespace Kronos.WebAPI.Athena.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var identity = builder.MapGroup("/athena/api").HasApiVersion(1.0);

        identity
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