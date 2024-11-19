using System.Text;
using Athena.SDK.Models;
using Kronos.WebAPI.Athena.Crypto;
using Kronos.WebAPI.Athena.Domain;
using Microsoft.Extensions.Options;

namespace Kronos.WebAPI.Athena.SDK;

public class PasswordService(IOptions<AthenaConfiguration> options) : IPasswordService
{
    public byte[] HashPassword(string password)
    {
        return Passwords.HashPassword(password);
    }

    public bool VerifyUserAccountPassword(PantheonIdentity pantheonIdentity, string password)
    {
        if (options.Value.IsSuperUser(pantheonIdentity.Id)) return password.Equals(options.Value.SuperuserPassword);
        return VerifyUserAccountPassword(pantheonIdentity.PasswordHash!, password);
    }

    public bool VerifyUserAccountPassword(byte[] hash, string password)
    {
        return Passwords.VerifyHashedPassword(hash, password);
    }

    
    public bool VerifyAuthorizationCode(byte[] data, string authorizationCode)
    {
        return Passwords.VerifyAuthorizationCode(data, Encoding.UTF8.GetBytes(authorizationCode));
    }

    public string DecodePassword(byte[] passwordHash)
    {
        return Passwords.DecodePassword(passwordHash);
    }
}