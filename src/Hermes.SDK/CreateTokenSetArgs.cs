using System;

namespace Hermes.SDK
{
    public sealed class CreateTokenSetArgs
    {
        public CredentialsType CredentialsType { get; set; }
        public string? Password { get; set; }
        public string[] RequestedScopes { get; set; } = Array.Empty<string>();
        public Guid Audience { get; set; }
        public string? DeviceId { get; set; }
        public string? Username { get; set; }
        public Guid? ServiceId { get; set; }
        public string? AuthorizationCode { get; set; }
    }
}