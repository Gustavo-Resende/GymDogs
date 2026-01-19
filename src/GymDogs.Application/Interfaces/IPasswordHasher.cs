namespace GymDogs.Application.Interfaces;

/// <summary>
/// Service to hash and verify passwords
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Creates a hash of the password
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies if the password matches the hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}