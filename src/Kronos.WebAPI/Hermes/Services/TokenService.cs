using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kronos.WebAPI.Hermes.WebApi;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kronos.WebAPI.Hermes.Services;

public sealed class TokenCreationArgs
{
    public string? DeviceId { get; set; }
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public bool IsVerified { get; set; }
}

public class TokenService(IOptions<JwtConfig> options)
{
    public string CreateAccessToken(TokenCreationArgs args)
    {
        var handler = new JwtSecurityTokenHandler();


        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key)),
            SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim?> enumerable =
        [
            string.IsNullOrWhiteSpace(args.DeviceId) ? null : new Claim(Definitions.ClaimTypes.DeviceId, args.DeviceId),
            string.IsNullOrWhiteSpace(args.Username) ? null : new Claim(JwtRegisteredClaimNames.Nickname, args.Username),
            new Claim(ClaimTypes.Name, args.UserId.ToString("N"))
        ];
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = new ClaimsIdentity(
                enumerable.Where(x => x is not null).Cast<Claim>()
            ),
            Audience = options.Value.Audience,
            Issuer = options.Value.Issuer
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}