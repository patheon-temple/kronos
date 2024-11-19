using Athena.SDK.Models;
using Kronos.WebAPI.Athena.Crypto;

namespace Kronos.WebAPI.Athena.Domain;

public sealed class AthenaConfiguration
{
    public required string SuperuserUsername { get; set; }
    public required string SuperuserPassword { get; set; }
    public required Guid SuperuserId { get; set; }
    public byte[] SuperuserPasswordEncoded => Passwords.EncodePassword(SuperuserPassword);

    public bool IsSuperUser(Guid id) => id.Equals(SuperuserId);

    public bool IsSuperUser(string username) => SuperuserUsername.Equals(username);
   
    
}