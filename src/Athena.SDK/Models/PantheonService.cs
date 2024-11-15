using System;

namespace Athena.SDK.Models
{
    public class PantheonService
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[] AuthorizationCode { get; set; } = Array.Empty<byte>();
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }
}