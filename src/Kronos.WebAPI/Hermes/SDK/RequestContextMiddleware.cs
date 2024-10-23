using System.IdentityModel.Tokens.Jwt;

namespace Kronos.WebAPI.Hermes.SDK;

public sealed class PantheonRequestContext
{
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
}

public static class RequestContextMiddleware
{
    public static void UsePantheonRequestContext(this IApplicationBuilder builder)
    {
        builder.Use((context, @delegate) =>
        {
            var principal = context.RequestServices.GetRequiredService<IHttpContextAccessor>()
                .HttpContext?
                .User;
            var identityName = principal?.Identity?
                .Name;

            if (identityName is null || !Guid.TryParse(identityName, out var id)) return @delegate.Invoke();
            var requestContext = context.RequestServices.GetRequiredService<PantheonRequestContext>();
            requestContext.UserId = id;
            requestContext.Username = principal?.Claims.FirstOrDefault(x=>x.Type.Equals(JwtRegisteredClaimNames.Nickname))?.Value;

            return @delegate.Invoke();
        });
    }
}