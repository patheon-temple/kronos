namespace Kronos.WebAPI.Athena.WebApi.Interop.Responses;

public class GetAudienceCryptoResponse
{
    public required string SigningKey { get; init; }
    public required string EncryptionKey { get; init; }
    public required double Expiration { get; init; }
}