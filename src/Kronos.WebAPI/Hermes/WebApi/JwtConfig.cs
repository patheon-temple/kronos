using System.ComponentModel.DataAnnotations;
using System.Text;
using Hermes.SDK;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.WebApi;

public sealed class JwtConfig 
{
    [Required] public required string Issuer { get; set; }
    [Required] public required string SigningKey { get; set; }
    [Required] public required string EncryptionKey { get; set; }

    public TokenCryptoData ToTokenCryptoData() => new()
    {
        EncryptionKey = Convert.FromBase64String(EncryptionKey),
        SigningKey = Convert.FromBase64String(SigningKey),
        EntityId = GlobalDefinitions.Jwt.AthenaAudience
    };
}