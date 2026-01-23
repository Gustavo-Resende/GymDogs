namespace GymDogs.Application.Users.Dtos;

/// <summary>
/// DTO returned after successful token refresh
/// </summary>
public record RefreshTokenDto
{
    /// <summary>
    /// New JWT access token
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// New refresh token
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Access token expiration date and time (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Refresh token expiration date and time (UTC)
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; init; }
}
