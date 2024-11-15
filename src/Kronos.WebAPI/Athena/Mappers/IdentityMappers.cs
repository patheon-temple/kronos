using Athena.SDK.Models;
using Kronos.WebAPI.Athena.Data;

namespace Kronos.WebAPI.Athena.Mappers;

public static class IdentityMappers
{
    public static PantheonIdentity ToDomain(UserAccountDataModel data) => new()
    {
        DeviceId = data.DeviceId,
        Id = data.Id,
        PasswordHash = data.PasswordHash,
        Username = data.Username,
        Scopes = data.Scopes.Select(x => x.Id).ToArray(),
    };
}

public static class ServiceMappers
{
    public static PantheonService ToDomain(ServiceAccountDataModel data) => new()
    {
        Id = data.Id,
        Name = data.Name,
        AuthorizationCode = data.AuthorizationCode,
        Scopes = data.Scopes.Select(x => x.Id).ToArray(),
    };
}