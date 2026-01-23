namespace GymDogs.Application.Users.Dtos;

/// <summary>
/// DTO returned after successful login containing JWT token and user information
/// </summary>
public record LoginDto
{
    /// <summary>
    /// JWT access token to be used in subsequent requests
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Refresh token to be used to obtain a new access token
    /// </summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// User unique identifier
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Username
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Access token expiration date and time (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Refresh token expiration date and time (UTC)
    /// </summary>
    public DateTime RefreshTokenExpiresAt { get; init; }

    /// <summary>
    /// User role
    /// </summary>
    public string Role { get; init; } = string.Empty;
}