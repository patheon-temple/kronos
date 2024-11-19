using Athena.SDK;
using Athena.SDK.Models;
using FluentValidation;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Data;
using Kronos.WebAPI.Athena.Mappers;

namespace Kronos.WebAPI.Athena.SDK;

internal sealed class AthenaApi(
    IPasswordService passwordService,
    IAthenaRepository athenaRepository) : IAthenaApi
{
    public async Task<PantheonIdentity> CreateUserFromDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default)
    {
        var identity = await athenaRepository.GetUserAccountByDeviceIdAsync(deviceId, cancellationToken);

        if (identity is not null) return IdentityMappers.ToDomain(identity);

        await athenaRepository.CreateUserAccountAsync(identity = new UserAccountDataModel
        {
            DeviceId = deviceId
        }, cancellationToken);

        return IdentityMappers.ToDomain(identity);
    }

    public async Task<PantheonIdentity?> GetUserByDeviceIdAsync(string deviceId,
        CancellationToken cancellationToken = default)
    {
        var entity = await athenaRepository.GetUserAccountByDeviceIdAsync(deviceId, cancellationToken);
        return entity is null ? null : IdentityMappers.ToDomain(entity);
    }

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

    public async Task<PantheonIdentity> CreateUserFromUsernameAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        username = username.ToLower();

        await IsUsernameAndPasswordValidOrThrowAsync(username, password, cancellationToken);
        UserAccountDataModel entity;
        await athenaRepository.CreateUserAccountAsync(entity = new UserAccountDataModel
        {
            DeviceId = null,
            Username = username,
            PasswordHash = passwordService.HashPassword(password)
        }, cancellationToken);

        return IdentityMappers.ToDomain(entity);
    }

    private async Task IsUsernameAndPasswordValidOrThrowAsync(string username, string password,
        CancellationToken cancellationToken)
    {
        if (await DoesUsernameExistAsync(username, cancellationToken))
            throw new ValidationException("Username already exists.");

        var passwordValidator = Passwords.CreateValidator();
        var validationResult = await passwordValidator.ValidateAsync(new UserCredentialsValidationParams
        {
            Username = username,
            Password = password
        }, cancellationToken);

        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);
    }

    public async Task<PantheonIdentity?> GetUserByUsernameAsync(string username,
        CancellationToken cancellationToken = default)
    {
        var data = await athenaRepository.GetUserAccountByUsernameAsync(username, cancellationToken);

        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<bool> ValidateUserCredentialsAsync(Guid userId, string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var user = await GetUserByIdAsync(userId, cancellationToken);

        return user != null && passwordService.VerifyUserAccountPassword(user, password);
    }

    public async Task<PantheonIdentity?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var data = await athenaRepository.GetUserAccountByIdAsync(userId, cancellationToken);
        return data is null ? null : IdentityMappers.ToDomain(data);
    }

    public async Task<bool> ValidateServiceCodeAsync(Guid serviceId, string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        var data = await athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);
        return data is not null && passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode);
    }

    public async Task<string> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var data = await athenaRepository.GetUserAccountByIdAsync(userId, cancellationToken);

        if (data is null)
            throw new NullReferenceException("Not found");

        data.PasswordHash = passwordService.HashPassword(Guid.NewGuid().ToString("D"));

        await athenaRepository.UpdateUserAsync(data, cancellationToken);
        return passwordService.DecodePassword(data.PasswordHash);
    }

    public async Task<PantheonIdentity?> GetValidatedUserByDeviceIdAsync(string deviceId, string password,
        CancellationToken cancellationToken = default)
    {
        var identity = await athenaRepository.GetUserAccountByDeviceIdAsync(deviceId, cancellationToken);
        return GetValidatedIdentityAsync(identity, password);
    }

    private PantheonIdentity? GetValidatedIdentityAsync(UserAccountDataModel? identity, string password)
    {
        if (identity is null) return null;
        return passwordService.VerifyUserAccountPassword(identity.Id, identity.PasswordHash!, password)
            ? IdentityMappers.ToDomain(identity)
            : null;
    }

    public async Task<PantheonIdentity?> GetValidatedUserByUsernameAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        var identity = await athenaRepository.GetUserAccountByUsernameAsync(username, cancellationToken);
        return GetValidatedIdentityAsync(identity, password);
    }

    public async Task<PantheonService?> GetValidatedServiceAsync(Guid serviceId, string authorizationCode,
        CancellationToken cancellationToken = default)
    {
        var data = await GetServiceAccountByIdAsync(serviceId, cancellationToken);
        return data is not null && passwordService.VerifyAuthorizationCode(data.AuthorizationCode, authorizationCode)
            ? data
            : null;
    }

    private async Task<PantheonService?> GetServiceAccountByIdAsync(Guid serviceId, CancellationToken cancellationToken)
    {
        var data = await athenaRepository.GetServiceAccountAsync(serviceId, cancellationToken);
        return data == null ? null : ServiceMappers.ToDomain(data);
    }
}