using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hermes.SDK
{
    public enum CreateTokenSetResult
    {
        Success = 1,
        Failure = 2,
    }

    public sealed class CreateTokenSetArgs
    {
        public CredentialsType CredentialsType { get; set; }
        public string? Password { get; set; }
        public string[] RequestedScopes { get; set; } = Array.Empty<string>();
        public Guid Audience { get; set; }
        public string? DeviceId { get; set; }
        public string? Username { get; set; }
        public Guid? ServiceId { get; set; }
        public string? AuthorizationCode { get; set; }
    }

    public interface IHermesApi
    {
        Task<Result<CreateTokenSetResult, TokenSet>> CreateTokenSetAsync(CreateTokenSetArgs args,
            CancellationToken cancellationToken);

        Task<Result<CreateTokenSetResult, TokenSet>> CreateTokenSetForDeviceAsync(string deviceId, string password, string[] requestedScopes,
            Guid audience,
            CancellationToken cancellationToken);

        Task<Result<CreateTokenSetResult, TokenSet>> CreateTokenSetForUserCredentialsAsync(string username, string password, string[] requestedScopes,
            Guid audience,
            CancellationToken cancellationToken);

        Task<Result<CreateTokenSetResult, TokenSet>> CreateTokenSetForServiceAsync(Guid serviceId, string authorizationCode, string[] requestedScopes,
            Guid audience, CancellationToken cancellationToken = default);

    }
}