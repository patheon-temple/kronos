using Kronos.WebAPI.Athena.Data;

namespace Kronos.WebAPI.Athena.SDK
{
    public interface IAthenaRepository
    {
        Task<UserAccountDataModel?> GetUserAccountByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);
        Task CreateUserAccountAsync(UserAccountDataModel identity, CancellationToken cancellationToken);
        Task<bool> DoesUsernameExistsAsync(string username, CancellationToken cancellationToken);
        Task<UserAccountDataModel?> GetUserAccountByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<UserAccountDataModel?> GetUserAccountByIdAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateUserAsync(UserAccountDataModel data, CancellationToken cancellationToken = default);
        Task<ServiceAccountDataModel?> GetServiceAccountAsync(Guid serviceId,
            CancellationToken cancellationToken = default);
    }

}