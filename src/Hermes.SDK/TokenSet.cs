
using System;

namespace Hermes.SDK
{
    public sealed class TokenSet
    {
        public string AccessToken { get; set; } = string.Empty;
        public  string UserId { get; set; }= string.Empty;
        public string[] Scopes { get; set; } = Array.Empty<string>();
        public string? Username { get; set; }
    }
}