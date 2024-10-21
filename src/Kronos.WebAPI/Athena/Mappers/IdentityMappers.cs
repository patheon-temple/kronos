using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Athena.Mappers;

public static class IdentityMappers
{
    public static PantheonIdentity ToDomain(PantheonIdentityDataModel data) => new()
    {
        DeviceId = data.DeviceId,
        Id = data.Id
    };
}