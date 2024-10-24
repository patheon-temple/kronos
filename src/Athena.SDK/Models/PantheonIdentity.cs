using System;

namespace Athena.SDK.Models
{
    public class PantheonIdentity
    {
        public Guid Id { get; set; }
    
        public string? DeviceId { get; set; }
        public byte[]? PasswordHash { get; set; }
    }
}