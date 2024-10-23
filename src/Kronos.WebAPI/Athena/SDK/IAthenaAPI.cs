using Kronos.WebAPI.Athena.Domain;

namespace Kronos.WebAPI.Athena.SDK;

public interface IAthenaApi
{
    Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default);

    Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);

    Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default);

    Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password,
        CancellationToken stoppingToken = default);

    Task<PantheonIdentity?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> VerifyPasswordAsync(byte[] passwordHash, string password, CancellationToken cancellationToken);
}