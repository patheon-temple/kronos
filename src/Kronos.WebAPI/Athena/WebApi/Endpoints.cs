using FluentValidation;
using Kronos.WebAPI.Athena.SDK;
using Kronos.WebAPI.Athena.WebApi.Interop.Requests;
using Kronos.WebAPI.Athena.WebApi.Interop.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kronos.WebAPI.Athena.WebApi;

public static class Endpoints
{
    public static void Register(WebApplication app)
    {
        var builder = app.NewVersionedApi("Athena");
        var identity = builder.MapGroup("/athena/api").HasApiVersion(1.0);

        identity
            .MapGet("/identities/me", Identities.GetMe)
            .Produces<CreateDeviceIdentityResponse>()
            .Produces(400)
            .ProducesValidationProblem()
            .MapToApiVersion(1.0)
            .RequireAuthorization();
        identity
            .MapPost("/identities/authenticate/device", Identities.PostDeviceIdentityAsync)
            .Produces<CreateDeviceIdentityResponse>()
            .Produces(400)
            .ProducesValidationProblem()
            .MapToApiVersion(1.0);
    }
}

internal static class Identities
{
    public static IResult GetMe()
    {
        return Results.Empty;
    }

    public static async Task<IResult> PostDeviceIdentityAsync(
        [FromBody] CreateDeviceIdentityRequest request,
        [FromServices] IValidator<CreateDeviceIdentityRequest> validator,
        [FromServices] IAthenaApi athenaApi)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToDictionary());
        }
        
        var identity = await athenaApi.CreateIdentityByDeviceIdAsync(request.DeviceId);
        return Results.Ok(new CreateDeviceIdentityResponse
        {
            Id = identity.Id,
            DeviceId = identity.DeviceId
        });
    }
}