using Hermes.SDK;
using Kronos.WebAPI.Hermes.SDK;

namespace Kronos.WebAPI.Hermes.Mappers;

public static class HermesMappers
{
    public static TokenCryptoData ToDomain(TokenCryptoDataModel dataModel)
    {
        return new TokenCryptoData
        {
            EncryptionKey = dataModel.EncryptionKey,
            EntityId = dataModel.EntityId,
            SigningKey = dataModel.SigningKey
        };
    }

}