namespace Kronos.WebAPI.Hermes.Services;

public sealed class TokenUserData
{
    public string? Username { get; set; }
    public Guid Id { get; set; }
    public string[] Scopes { get; set; } = [];
    public required string Audience { get; set; }
}