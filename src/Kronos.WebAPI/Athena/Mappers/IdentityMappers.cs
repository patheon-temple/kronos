using Athena.SDK.Models;
using Kronos.WebAPI.Athena.Data;

namespace Kronos.WebAPI.Athena.Mappers;

public static class IdentityMappers
{
    public static PantheonIdentity ToDomain(UserAccountDataModel data) => new()
    {
        DeviceId = data.DeviceId,
        Id = data.UserId,
        PasswordHash = data.PasswordHash,
        Username = data.Username,
        Scopes = data.Scopes.Select(x => x.Id).ToArray(),
    };
}