using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(TokenService tokenService) : IHermesApi
{
    public TokenSet CreateTokenSetForIdentity(PantheonIdentity identity)
    {
        var accessToken = tokenService.CreateAccessToken(new TokenCreationArgs
        {
            DeviceId = identity.DeviceId,
            UserId = identity.Id
        });

        return new TokenSet
        {
            AccessToken = accessToken
        };
    }
}