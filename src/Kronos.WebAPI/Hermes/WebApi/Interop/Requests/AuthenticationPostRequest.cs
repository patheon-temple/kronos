using FluentValidation;

namespace Kronos.WebAPI.Hermes.WebApi.Interop.Requests;

public sealed class AuthenticationPostRequest
{
    public sealed class Validator : AbstractValidator<AuthenticationPostRequest>
    {
        public Validator()
        {
            When(x => x.CredentialsType == CredentialsType.DeviceId,
                () =>
                {
                    RuleFor(x => x.DeviceId)
                        .NotNull()
                        .MinimumLength(12)
                        .MaximumLength(128);
                });
            
            When(x => x.CredentialsType == CredentialsType.Password,
                () =>
                {
                    RuleFor(x => x.Username)
                        .NotNull()
                        .MinimumLength(6)
                        .MaximumLength(128);
                    RuleFor(x => x.Password)
                        .NotNull()
                        .MinimumLength(6)
                        .MaximumLength(128);
                });
        }
    }

    /// <summary>
    /// Type of credentials
    /// </summary>
    /// <example>1</example>
    public required CredentialsType CredentialsType { get; set; } = CredentialsType.DeviceId;

    /// <summary>
    /// Unique device ID
    /// </summary>
    /// <example>00-B0-D0-63-C2-26</example>
    public string? DeviceId { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }
}