using Hermes.SDK;

namespace Kronos.WebAPI.Hermes.UseCases
{
    public sealed class CreateTokenSetUseCase
    {
        private readonly IHermesApi _hermesApi;

        public CreateTokenSetUseCase(IHermesApi hermesApi)
        {
            _hermesApi = hermesApi;
        }

        public sealed class InputParameters
        {
            public InputParameters(Guid audience)
            {
                Audience = audience;
            }

            public CredentialsType CredentialsType { get; set; }
            public string? Password { get; set; }
            public string[] RequestedScopes { get; set; } = Array.Empty<string>();
            public Guid Audience { get; set; }
            public string? DeviceId { get; set; }
            public string? Username { get; set; }
            public Guid? ServiceId { get; set; }
            public string? AuthorizationCode { get; set; }
        }


        public enum ResultType
        {
            Unknown = 0,
            Success = 1,
            Failure = 2,
        }

        public async Task<Result<ResultType, TokenSet>> ExecuteAsync(InputParameters request,
            CancellationToken cancellationToken = default)
        {
            var tokenSet = await GetTokenSet(request, cancellationToken);
            return new Result<ResultType, TokenSet>(ResultType.Success, tokenSet);
        }

        private async Task<TokenSet> GetTokenSet(InputParameters request, CancellationToken cancellationToken)
        {
            return request.CredentialsType switch
            {
                CredentialsType.DeviceId => await _hermesApi.CreateTokenSetForDeviceAsync(request.DeviceId!,
                    request.Password!, request.RequestedScopes.ToArray(), request.Audience, cancellationToken),
                CredentialsType.Password => await _hermesApi.CreateTokenSetForUserCredentialsAsync(request.Username!,
                    request.Password!, request.RequestedScopes.ToArray(), request.Audience, cancellationToken),
                CredentialsType.AuthorizationCode => await _hermesApi.CreateTokenSetForServiceAsync(
                    request.ServiceId!.Value,
                    request.AuthorizationCode!, request.RequestedScopes, request.Audience,
                    cancellationToken),
                CredentialsType.Unknown => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}