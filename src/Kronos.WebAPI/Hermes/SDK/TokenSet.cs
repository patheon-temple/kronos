namespace Kronos.WebAPI.Hermes.SDK;

public sealed class TokenSet
{
    public required string AccessToken { get; set; }
    public required string UserId { get; set; }
    public required string[] Scopes { get; set; }
    public string? Username { get; set; }
}