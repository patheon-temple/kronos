using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Athena.Mappers;

public static class IdentityMappers
{
    public static PantheonIdentity ToDomain(UserAccountDataModel data) => new()
    {
        DeviceId = data.DeviceId,
        Id = data.UserId
    };
}