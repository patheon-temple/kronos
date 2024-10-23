using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Kronos.WebAPI.Athena.Crypto;

public static partial class Passwords
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int IterationsCount = 1000;
    private const int Offset = 1;
    private static int HashSize => Offset + SaltSize + KeySize;

    public static byte[] HashPassword(string password)
    {
        byte[] salt;
        byte[] key;

        ArgumentNullException.ThrowIfNull(password);

        using (var bytes = new Rfc2898DeriveBytes(password, SaltSize, IterationsCount, HashAlgorithmName.SHA512))
        {
            salt = bytes.Salt;
            key = bytes.GetBytes(KeySize);
        }

        var dst = new byte[HashSize];
        Buffer.BlockCopy(salt, 0, dst, Offset, SaltSize);
        Buffer.BlockCopy(key, 0, dst, SaltSize + Offset, KeySize);
        return dst;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storedPassword">Base64 hashed string</param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool VerifyHashedPassword(byte[] storedPassword, string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(storedPassword);

        if (storedPassword.Length != HashSize || storedPassword[0] != 0)
        {
            return false;
        }

        var storedSalt = new byte[SaltSize];
        var storedKey = new byte[KeySize];

        Buffer.BlockCopy(storedPassword, Offset, storedSalt, 0, SaltSize);
        Buffer.BlockCopy(storedPassword, Offset + SaltSize, storedKey, 0, KeySize);

        using var bytes = new Rfc2898DeriveBytes(password, storedSalt, 1000, HashAlgorithmName.SHA512);

        var generatedKey = bytes.GetBytes(KeySize);

        return storedKey.SequenceEqual(generatedKey);
    }

    [GeneratedRegex(@"^ # start of line
(?=(?:.*[A-Z]){2,}) # 2 upper case letters
(?=(?:.*[a-z]){2,}) # 2 lower case letters
(?=(?:.*\d){2,}) # 2 digits
(?=(?:.*[!@#$%^&*()\-_=+{};:,<.>]){2,}) # 2 special characters
(?!.*(.)\1{2}) # negative lookahead, dont allow more than 2 repeating characters
([A-Za-z0-9!@#$%^&*()\-_=+{};:,<.>]{12,20}) # length 12-20, only above char classes (disallow spaces)
$ # end of line", RegexOptions.Compiled)]
    private static partial Regex ComplexityRegex();
}