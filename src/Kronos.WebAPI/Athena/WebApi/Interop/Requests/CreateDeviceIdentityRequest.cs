using FluentValidation;

namespace Kronos.WebAPI.Athena.WebApi.Interop.Requests;

public sealed class CreateDeviceIdentityRequest
{
    /// <summary>
    /// ID of device used to identify unique player
    /// </summary>
    /// <example>00-B0-D0-63-C2-26</example>
    public required string DeviceId { get; set; }

    public sealed class Validator : AbstractValidator<CreateDeviceIdentityRequest>
    {
        public Validator()
        {
            RuleFor(x => x.DeviceId).NotNull()
                .MinimumLength(12)
                .MaximumLength(128);
        }
    }
}