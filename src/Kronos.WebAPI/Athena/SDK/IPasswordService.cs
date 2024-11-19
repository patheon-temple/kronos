using Athena.SDK.Models;

namespace Kronos.WebAPI.Athena.SDK;

public interface IPasswordService
{
    byte[] HashPassword(string password);
    bool VerifyUserAccountPassword(PantheonIdentity pantheonIdentity, string password);
    bool VerifyAuthorizationCode(byte[] data, string authorizationCode);
}