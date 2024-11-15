using System;

namespace Hermes.SDK
{
    public class TokenCryptoData
    {
        public  Guid EntityId { get; set; }
        public  byte[] SigningKey { get; set; } = Array.Empty<byte>();
        public  byte[] EncryptionKey { get; set; } = Array.Empty<byte>();
        public double Expiration { get; set; } = TimeSpan.FromDays(14).TotalSeconds;
    }
}