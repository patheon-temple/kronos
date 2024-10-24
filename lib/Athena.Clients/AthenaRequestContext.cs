using System;
using Grpc.Core;

namespace Pantheon.Athena.Clients
{
    public sealed class AthenaRequestContext
    {
        public TimeSpan CallTimeout { get; } = TimeSpan.FromSeconds(2);
        public DateTime CallDeadline => DateTime.UtcNow.Add(CallTimeout);
        
        private string? AccessToken { get; set; }

        public Metadata CreateMetadata()
        {
            var metadata = new Metadata();
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                metadata.Add("Authorization", $"Bearer {AccessToken}");
            }

            return metadata;
        }
    }
}