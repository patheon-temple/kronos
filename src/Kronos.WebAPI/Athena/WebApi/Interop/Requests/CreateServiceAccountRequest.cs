using System.ComponentModel.DataAnnotations;

namespace Kronos.WebAPI.Athena.WebApi.Interop.Requests;

internal class CreateServiceAccountRequest
{
    [MaxLength(256)]
    public required string ServiceName { get; set; }
    
    public string[] Scopes { get; set; } = Array.Empty<string>();
}