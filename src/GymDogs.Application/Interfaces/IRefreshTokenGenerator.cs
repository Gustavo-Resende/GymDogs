namespace GymDogs.Application.Interfaces;

/// <summary>
/// Service to generate refresh tokens for authentication
/// </summary>
public interface IRefreshTokenGenerator
{
    /// <summary>
    /// Generates a refresh token string
    /// </summary>
    /// <returns>Refresh token as string</returns>
    string GenerateRefreshToken();
}
