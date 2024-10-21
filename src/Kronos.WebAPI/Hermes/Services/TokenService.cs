using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Kronos.WebAPI.Hermes.Services;

public sealed class TokenCreationArgs
{
    public string? DeviceId { get; set; }
    public Guid UserId { get; set; }
}

public class TokenService
{
    public string CreateAccessToken(TokenCreationArgs args)
    {
        var handler = new JwtSecurityTokenHandler();

        var privateKey = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY") ?? throw new Exception("Missing JWT_PRIVATE_KEY");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)),
            SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim?> enumerable =
        [
            string.IsNullOrWhiteSpace(args.DeviceId) ? null : new Claim(Definitions.ClaimTypes.DeviceId, args.DeviceId),
            new Claim(ClaimTypes.Name, args.UserId.ToString("N"))
        ];
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = new ClaimsIdentity(
                enumerable.Where(x => x is not null).Cast<Claim>()
            )
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}