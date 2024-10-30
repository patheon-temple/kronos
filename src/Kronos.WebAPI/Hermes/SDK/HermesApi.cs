using System.Security;
using Athena.SDK;
using Kronos.WebAPI.Hermes.Exceptions;
using Kronos.WebAPI.Hermes.Services;

namespace Kronos.WebAPI.Hermes.SDK;

internal class HermesApi(IAthenaApi athenaApi, TokenService tokenService) : IHermesApi
{
    public async Task<TokenSet> CreateTokenSetForDeviceAsync(string deviceId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceId);
        var identity = await athenaApi.GetUserByDeviceIdAsync(deviceId, cancellationToken) ??
                       await athenaApi.CreateUserFromDeviceIdAsync(deviceId, CancellationToken.None);

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

    public async Task<TokenSet> CreateTokenSetForUserCredentialsAsync(string username, string password,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var identity = await athenaApi.GetUserByUsernameAsync(username, cancellationToken);

        if (identity is null)
            throw new UserNotFoundExistException
            {
                Data = { { "username", username } }
            };

        if (!await athenaApi.VerifyPasswordAsync(identity.Id, password, cancellationToken))
        {
            throw new SecurityException();
        }

        var accessToken = tokenService.CreateAccessToken(new TokenCreationArgs
        {
            UserId = identity.Id,
            Username = username,
            IsVerified = false
        });

        return new TokenSet
        {
            AccessToken = accessToken
        };
    }
}