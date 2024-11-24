namespace Kronos.WebAPI.Hermes.Extensions;

public static class HttpContextExtensions
{
    public static bool HasRequestHeaderValue(this IHttpContextAccessor contextAccessor, string headName,
        string valueToCheck) =>
        contextAccessor.HasRequestHeaderValue(headName, x => !string.IsNullOrWhiteSpace(x));

    public static bool HasRequestHeaderValue(this IHttpContextAccessor contextAccessor, string headName,
        Func<string?, bool> predicate)
    {
        if (contextAccessor.HttpContext is null) return false;
        if (!contextAccessor.HttpContext.Request.Headers.TryGetValue(GlobalDefinitions.Headers.ValidateOnly,
                out var value))
        {
            return false;
        }

        var valueStr = value.ToString();
        return predicate(valueStr);
    }
}