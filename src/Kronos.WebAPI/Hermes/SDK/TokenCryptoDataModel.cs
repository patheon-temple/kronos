namespace Kronos.WebAPI.Hermes.SDK;

public class TokenCryptoDataModel
{
    public required Guid EntityId { get; set; }
    public required byte[] SigningKey { get; set; }
    public required byte[] EncryptionKey { get; set; }
}