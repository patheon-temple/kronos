using Athena.SDK.Models;
using Google.Protobuf;
using Kronos.WebAPI.Athena.Data;

namespace Kronos.WebAPI.Athena.Mappers;

public static class IdentityMappers
{
    public static PantheonIdentity ToDomain(UserAccountDataModel data) => new()
    {
        DeviceId = data.DeviceId,
        Id = data.UserId,
        PasswordHash = data.PasswordHash
    };

    public static Pantheon.Athena.Grpc.Common.PantheonIdentity ToGrpc(PantheonIdentity identity) =>
        new()
        {
            Id = ByteString.CopyFrom(identity.Id.ToByteArray()),
            DeviceId = identity.DeviceId ?? string.Empty,
        };
}