using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.SDK;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(IAthenaApi athenaApi, TokenService tokenService) : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentNullException(nameof(deviceId));
                        
        var identity = await athenaApi.GetIdentityByDeviceIdAsync(deviceId, cancellationToken) ??
                       await athenaApi.CreateIdentityByDeviceIdAsync(deviceId, CancellationToken.None);

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