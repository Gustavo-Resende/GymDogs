namespace GymDogs.Application.Interfaces;

/// <summary>
/// Service to generate JWT tokens for authentication
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the given user ID, username and email
    /// </summary>
    /// <param name="userId">Unique identifier of the user</param>
    /// <param name="username">Username of the user</param>
    /// <param name="email">Email of the user</param>
    /// <returns>JWT token as string</returns>
    string GenerateToken(Guid userId, string username, string email);
}
