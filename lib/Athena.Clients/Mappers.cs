using System;
using Athena.SDK.Models;

namespace Pantheon.Athena.Clients
{
    public static class Mappers
    {
        public static PantheonIdentity FromGrpc(Pantheon.Athena.Grpc.PantheonIdentity identity)
        {
            return new PantheonIdentity
            {
                DeviceId = identity.DeviceId,
                Id = new Guid(identity.Id.ToByteArray()),
                PasswordHash = null
            };
        }
    }
}