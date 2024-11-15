using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Athena.SDK.Models;
using Hermes.SDK;
using Microsoft.IdentityModel.Tokens;

namespace Kronos.WebAPI.Hermes.Services;

public abstract class TokenService
{
    public static string CreateAccessToken(TokenUserData args, TokenCryptoData cryptoData,
        GlobalDefinitions.AccountType accountType)
    {
        var handler = new JwtSecurityTokenHandler();


        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(cryptoData.SigningKey),
            SecurityAlgorithms.HmacSha512);

        IEnumerable<Claim?> enumerable =
        [
            string.IsNullOrWhiteSpace(args.Username)
                ? null
                : new Claim(JwtRegisteredClaimNames.Nickname, args.Username),
            new(ClaimTypes.Name, args.Id.ToString("N")),
            new(GlobalDefinitions.ClaimTypes.AccountType, accountType.ToString()),
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddSeconds(cryptoData.Expiration),
            Subject = new ClaimsIdentity(
                enumerable.Where(x => x is not null).Cast<Claim>()
                    .Union(args.Scopes.Select(x => new Claim(ClaimTypes.Role, x)))
            ),
            Audience = cryptoData.EntityId.ToString("N"),
            Issuer = GlobalDefinitions.Jwt.Issuer,
            IssuedAt = DateTime.UtcNow,
            EncryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(cryptoData.EncryptionKey),
                SecurityAlgorithms.Aes256KW,SecurityAlgorithms.Aes256CbcHmacSha512),
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}