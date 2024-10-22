using Kronos.WebAPI.Athena.Domain;
using Kronos.WebAPI.Athena.SDK;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Athena.WebApi.HostedServices;

public sealed class SuperuserHostedService(
    IOptions<AthenaConfiguration> options,
    IAthenaApi athenaApi,
    ILogger<SuperuserHostedService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var superuser = options.Value.Superuser;
        if (string.IsNullOrWhiteSpace(superuser)) return;

        var credentials = superuser.Split('@');
        if (credentials.Length != 2)
        {
            logger.LogError("Credentials are not in valid format ****@****, instead found {Value}", superuser);
            return;
        }

        var username = credentials[0];
        var password = credentials[1];
        if(string.IsNullOrWhiteSpace(username)) throw new NullReferenceException(nameof(username));
        if(string.IsNullOrWhiteSpace(password)) throw new NullReferenceException(nameof(password));
        var exists = await athenaApi.DoesUsernameExistAsync(username, stoppingToken);
        if (exists)
        {
            logger.LogInformation("Superuser exists!");
            return;
        }
        
        await athenaApi.CreateUserFromUsernameAsync(username, password, stoppingToken);
    }
}