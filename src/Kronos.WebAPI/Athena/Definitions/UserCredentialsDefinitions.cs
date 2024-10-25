﻿namespace Kronos.WebAPI.Athena.Definitions;

public sealed class UserCredentialsDefinitions
{
    public const int UsernameMaxLength = 32;
    public const int UsernameMinLength = 3;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;
}