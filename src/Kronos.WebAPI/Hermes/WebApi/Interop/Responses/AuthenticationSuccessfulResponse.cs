namespace Kronos.WebAPI.Hermes.WebApi.Interop.Responses;

public record AuthenticationSuccessfulResponse(string AccessToken, string UserId, string[] Scopes, string? Username);