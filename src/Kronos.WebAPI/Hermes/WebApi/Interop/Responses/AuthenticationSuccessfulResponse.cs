namespace Kronos.WebAPI.Hermes.WebApi.Interop.Responses;

public sealed class AuthenticationSuccessfulResponse
{
    public required string AccessToken { get; init; }
    public required string Id { get; set; }
    public string? Username { get; set; }
    public string[] Scopes { get; set; } = [];
}