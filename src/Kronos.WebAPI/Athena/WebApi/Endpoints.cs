using System.Text;
using Athena.SDK;
using Kronos.WebAPI.Athena.WebApi.Interop.Requests;
using Kronos.WebAPI.Athena.WebApi.Interop.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kronos.WebAPI.Athena.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var v1Admin = builder.MapGroup("/athena/api/admin/v{v:apiVersion}").HasApiVersion(1.0);
        v1Admin.MapPost("account/user", CreateUserAccount).RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
        v1Admin.MapPost("account/service",CreateServiceAccountAsync).RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
        v1Admin.MapGet("account/user/{id:guid}", GetUserAccountByIdAsync)
            .WithName(nameof(GetUserAccountByIdAsync)) // (mobert): added due to Results.Create routing
            .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
        v1Admin.MapGet("account/service/{id:guid}", GetServiceAccountByIdAsync)
            .WithName(nameof(GetServiceAccountByIdAsync)) // (mobert): added due to Results.Create routing
            .RequireAuthorization(GlobalDefinitions.Policies.SuperUser);
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
        return Results.CreatedAtRoute(nameof(GetUserAccountByIdAsync), new { id = identity.Id },
            new PantheonIdentity(identity));
    }

    private static async Task<IResult> CreateServiceAccountAsync(
        [FromBody] CreateServiceAccountRequest request,
        [FromServices] IAthenaAdminApi adminApi, CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.CreateServiceAccountAsync(request.ServiceName, request.Scopes, cancellationToken);
        return Results.CreatedAtRoute(nameof(GetUserAccountByIdAsync), new { id = identity.Id },
            new ServiceAccount
            {
                ServiceName = identity.Name,
                Id = identity.Id,
                AuthorizationCode = Encoding.UTF8.GetString(identity.Secret)
            });
    }

    private static async Task<IResult> GetServiceAccountByIdAsync(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] IAthenaAdminApi adminApi, CancellationToken cancellationToken = default)
    {
        var identity = await adminApi.GetServiceAccountByIdAsync(id, cancellationToken);
        if (identity is null) return Results.NoContent();
        return Results.Ok(new ServiceAccount
        {
            ServiceName = identity.Name,
            Id = identity.Id
        });
    }
}