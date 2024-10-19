namespace Zeus.Interop.Responses;

public sealed class AuthenticationSuccessfulResponse
{
    public required string AccessToken { get; init; }
}