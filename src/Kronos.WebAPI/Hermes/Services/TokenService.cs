using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Athena.SDK.Models;
using Kronos.WebAPI.Hermes.WebApi;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kronos.WebAPI.Hermes.Services;

public class TokenService(IOptions<JwtConfig> options)
{
    public string CreateAccessToken(PantheonIdentity args, string[] requestedScopes)
    {
        var handler = new JwtSecurityTokenHandler();


        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)),
            SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim?> enumerable =
        [
            string.IsNullOrWhiteSpace(args.Username)
                ? null
                : new Claim(JwtRegisteredClaimNames.Nickname, args.Username),
            new(ClaimTypes.Name, args.Id.ToString("N"))
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = new ClaimsIdentity(
                enumerable.Where(x => x is not null).Cast<Claim>()
                    .Union(args.Scopes.Where(requestedScopes.Contains).Select(x => new Claim(ClaimTypes.Role, x)))
            ),
            Audience = options.Value.Audience,
            Issuer = options.Value.Issuer
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}