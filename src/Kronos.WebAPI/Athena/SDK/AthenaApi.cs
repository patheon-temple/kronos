using Athena.SDK;
using Athena.SDK.Models;
using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Mappers;

namespace Kronos.WebAPI.Athena.SDK;

internal sealed class AthenaApi(
    IPasswordService passwordService,
    IAthenaRepository athenaRepository,
    ILogger<AthenaApi> logger) : IAthenaApi
{
    public Task<bool> DoesUsernameExistAsync(string username, CancellationToken cancellationToken = default)
    {
        username = username.ToLower();
        return athenaRepository.DoesUsernameExistsAsync(username, cancellationToken);
    }

    public async Task<bool> ValidateUserCredentialsByUsernameAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var data = await athenaRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
        return data is not null && passwordService.VerifyUserAccountPassword(data.Id, data.PasswordHash!, password);
    }

    public async Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        var data = await athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);
        return data is not null && passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode);
    }

    public async Task<(string? NewPassword, ResetUserPasswordError? Error)> ResetUserPasswordAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var data = await athenaRepository.GetUserAccountByIdAsync(userId, cancellationToken);

        if (data is null)
        {
            return (null, ResetUserPasswordError.InvalidPasswordFormat);
        }

        data.PasswordHash = passwordService.HashPassword(Guid.NewGuid().ToString("D"));

        await athenaRepository.UpdateUserAsync(data, cancellationToken);

        return (passwordService.DecodePassword(data.PasswordHash), null);
    }

    public async Task<(PantheonIdentity?, GetValidatedEntityError?)> GetValidatedUserByDeviceIdAsync(string deviceId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var identity = await athenaRepository.GetUserAccountByDeviceIdAsync(deviceId, cancellationToken);
        return GetValidatedIdentityAsync(password, identity);
    }


    public async Task<(PantheonIdentity?, GetValidatedEntityError?)> GetValidatedUserByUsernameAsync(string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var identity = await athenaRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
        return GetValidatedIdentityAsync(password, identity);
    }

    private (PantheonIdentity?, GetValidatedEntityError?) GetValidatedIdentityAsync(string password,
        UserAccountDataModel? identity)
    {
        if (identity is null) return (null, GetValidatedEntityError.IdentityNotFound);

        if (passwordService.VerifyUserAccountPassword(identity.Id, identity.PasswordHash!, password))
            return (IdentityMappers.ToDomain(identity), null);

        return (null, GetValidatedEntityError.InvalidCredentials);
    }

    public async Task<(PantheonService?, GetValidatedEntityError?)> GetValidatedServiceAsync(Guid serviceId,
        string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        var data = await GetServiceAccountByIdAsync(serviceId, cancellationToken);
        if (data is null) return (null, GetValidatedEntityError.IdentityNotFound);

        if (passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode))
            return (data, null);
        
        return (null, GetValidatedEntityError.InvalidCredentials);
    }

    private async Task<PantheonService?> GetServiceAccountByIdAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var data = await athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);
        
        return data == null ? null : ServiceMappers.ToDomain(data);
    }
}