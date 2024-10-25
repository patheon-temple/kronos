using Athena.SDK;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Domain;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Athena.WebApi.HostedServices;

public sealed class SuperuserHostedService(
    IOptions<AthenaConfiguration> options,
    IServiceProvider serviceProvider,
    ILogger<SuperuserHostedService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        try
        {
            var athenaApi = scope.ServiceProvider.GetRequiredService<IAthenaApi>();
            var superuser = options.Value.Superuser;
            if (string.IsNullOrWhiteSpace(superuser)) return;

            var username = new string(superuser.TakeWhile(x => x != '@').ToArray());

            if (string.IsNullOrWhiteSpace(username))
            {
                logger.LogError("Credentials are not in valid format ****@****, instead found {Value}", superuser);
                return;
            }

            var password = new string(superuser.Skip(username.Length + 1).ToArray());
            var validationResult = await Passwords.CreateValidator().ValidateAsync(new UserCredentialsValidationParams
            {
                Username = username,
                Password = password
            }, stoppingToken);
            if (!validationResult.IsValid)
                throw new Exception(
                    $"Invalid credentials: {string.Join("\n", validationResult.Errors.Select(x => x.ErrorMessage))}");

            var exists = await athenaApi.DoesUsernameExistAsync(username, stoppingToken);
            if (exists)
            {
                logger.LogInformation("Superuser exists!");
                return;
            }

            await athenaApi.CreateUserFromUsernameAsync(username, password, stoppingToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to run SuperuserHostedService");
        }
    }
}