namespace GymDogs.Application.Users.Dtos;

/// <summary>
/// DTO returned after successful login containing JWT token and user information
/// </summary>
public record LoginDto
{
    /// <summary>
    /// JWT token to be used in subsequent requests
    /// </summary>
    public string Token { get; init; } = string.Empty;

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
    /// Token expiration date and time (UTC)
    /// </summary>
    public DateTime ExpiresAt { get; init; }
}