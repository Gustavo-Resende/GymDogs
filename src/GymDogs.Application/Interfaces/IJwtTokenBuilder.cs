namespace GymDogs.Application.Interfaces;

/// <summary>
/// Builder interface for constructing JWT tokens in a fluent, step-by-step manner.
/// Implements the Builder Pattern to create complex JWT tokens with multiple claims and configurations.
/// </summary>
public interface IJwtTokenBuilder
{
    /// <summary>
    /// Sets the user ID claim for the token.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>The builder instance for method chaining</returns>
    IJwtTokenBuilder WithUserId(Guid userId);

    /// <summary>
    /// Sets the username claim for the token.
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <returns>The builder instance for method chaining</returns>
    IJwtTokenBuilder WithUsername(string username);

    /// <summary>
    /// Sets the email claim for the token.
    /// </summary>
    /// <param name="email">The email address of the user</param>
    /// <returns>The builder instance for method chaining</returns>
    IJwtTokenBuilder WithEmail(string email);

    /// <summary>
    /// Sets the role claim for the token.
    /// </summary>
    /// <param name="role">The role of the user (e.g., "Admin", "User")</param>
    /// <returns>The builder instance for method chaining</returns>
    IJwtTokenBuilder WithRole(string role);

    /// <summary>
    /// Sets the expiration time for the token in minutes.
    /// If not set, uses the default from configuration.
    /// </summary>
    /// <param name="minutes">Expiration time in minutes</param>
    /// <returns>The builder instance for method chaining</returns>
    IJwtTokenBuilder WithExpirationMinutes(int minutes);

    /// <summary>
    /// Adds a custom claim to the token.
    /// Can be called multiple times to add multiple custom claims.
    /// </summary>
    /// <param name="type">The claim type (key)</param>
    /// <param name="value">The claim value</param>
    /// <returns>The builder instance for method chaining</returns>
    IJwtTokenBuilder WithCustomClaim(string type, string value);

    /// <summary>
    /// Builds and returns the JWT token string.
    /// Validates required fields before building.
    /// </summary>
    /// <returns>The generated JWT token as a string</returns>
    /// <exception cref="InvalidOperationException">Thrown when required fields are missing</exception>
    string Build();
}
