using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace WebApi.Authorization
{

    static class PasswordHashing
    {
    public static string HashPassword(string password)
{
    // Generate a salt value
    byte[] salt = new byte[128 / 8];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(salt);
    }

    // Hash the password using the salt and KeyDerivation function
    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

    // Return the hashed password and salt as a string
    return $"{hashed}:{Convert.ToBase64String(salt)}";
}

public static bool VerifyPassword(string password, string hashedPasswordWithSalt)
{
    // Split the hashed password and salt
    string[] parts = hashedPasswordWithSalt.Split(':');
    if (parts.Length != 2)
    {
        return false;
    }
    byte[] salt = Convert.FromBase64String(parts[1]);

    // Hash the password with the salt
    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 10000,
        numBytesRequested: 256 / 8));

    // Compare the hashed password with the stored hashed password
    return parts[0] == hashed;
}
}
}