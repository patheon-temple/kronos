using Athena.SDK;
using Kronos.WebAPI.Athena.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Kronos.WebAPI.Athena.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var v1Admin = builder.MapGroup("/athena/api/admin/v{v:apiVersion}").HasApiVersion(1.0);
        v1Admin.MapPost("account/user", CreateUserAccount).RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
        v1Admin.MapGet("account/user/{id:guid}", GetUserAccountByIdAsync).WithName(nameof(GetUserAccountByIdAsync)).RequireAuthorization();
    }

    private static async Task<IResult> GetUserAccountByIdAsync(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] IAthenaAdminApi adminApi,
        CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.GetUserAccountByIdAsync(id, cancellationToken);
        if (identity is null)
            return Results.NoContent();
        return Results.Ok(new PantheonIdentity(identity));
    }


    private static async Task<IResult> CreateUserAccount(
        [FromBody] CreateUserAccountRequest request,
        [FromServices] IAthenaAdminApi adminApi, CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.CreateUserAsync(request.DeviceId, request.Username, request.Password,
            request.Scopes, cancellationToken);
        return Results.CreatedAtRoute(nameof(GetUserAccountByIdAsync), new { id = identity.Id }, new PantheonIdentity(identity));
    }
}

internal class CreateUserAccountRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? DeviceId { get; set; }
    public string[] Scopes { get; set; } = [];
}