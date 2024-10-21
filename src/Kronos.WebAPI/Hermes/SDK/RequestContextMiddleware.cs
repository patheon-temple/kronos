namespace Kronos.WebAPI.Hermes.SDK;

public sealed class PantheonRequestContext
{
    public Guid? UserId { get; set; }
}

public static class RequestContextMiddleware
{
    public static void UsePantheonRequestContext(this IApplicationBuilder builder)
    {
        builder.Use((context, @delegate) =>
        {
            var identityName = context.RequestServices.GetRequiredService<IHttpContextAccessor>()
                .HttpContext?
                .User
                .Identity?
                .Name;

            if (identityName is null || !Guid.TryParse(identityName, out var id)) return @delegate.Invoke();
            context.RequestServices.GetRequiredService<PantheonRequestContext>().UserId = id;

            return @delegate.Invoke();
        });
    }
}