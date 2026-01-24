using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GymDogs.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GymDogs.Infrastructure.Services;

/// <summary>
/// Builder implementation for constructing JWT tokens using the Builder Pattern.
/// Allows fluent, step-by-step construction of complex JWT tokens with validation.
/// </summary>
public class JwtTokenBuilder : IJwtTokenBuilder
{
    private Guid? _userId;
    private string? _username;
    private string? _email;
    private string? _role;
    private int? _expirationMinutes;
    private readonly List<Claim> _customClaims = new();
    private readonly IConfiguration _configuration;

    public JwtTokenBuilder(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IJwtTokenBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public IJwtTokenBuilder WithUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        _username = username;
        return this;
    }

    public IJwtTokenBuilder WithEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        _email = email;
        return this;
    }

    public IJwtTokenBuilder WithRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be null or empty", nameof(role));

        _role = role;
        return this;
    }

    public IJwtTokenBuilder WithExpirationMinutes(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentException("Expiration minutes must be greater than zero", nameof(minutes));

        _expirationMinutes = minutes;
        return this;
    }

    public IJwtTokenBuilder WithCustomClaim(string type, string value)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Claim type cannot be null or empty", nameof(type));

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Claim value cannot be null or empty", nameof(value));

        _customClaims.Add(new Claim(type, value));
        return this;
    }

    public string Build()
    {
        // Validate required fields
        if (!_userId.HasValue)
            throw new InvalidOperationException("UserId is required to build a JWT token");

        if (string.IsNullOrWhiteSpace(_username))
            throw new InvalidOperationException("Username is required to build a JWT token");

        if (string.IsNullOrWhiteSpace(_email))
            throw new InvalidOperationException("Email is required to build a JWT token");

        if (string.IsNullOrWhiteSpace(_role))
            throw new InvalidOperationException("Role is required to build a JWT token");

        // Get configuration
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");

        // Header information
        var issuer = jwtSettings["Issuer"] ?? "GymDogs";
        var audience = jwtSettings["Audience"] ?? "GymDogsUsers";
        var defaultExpirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "15");
        var tokenExpirationMinutes = _expirationMinutes ?? defaultExpirationMinutes;

        // Key information
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Build claims list
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.Value.ToString()),
            new Claim(ClaimTypes.Name, _username!),
            new Claim(ClaimTypes.Email, _email!),
            new Claim(ClaimTypes.Role, _role!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add custom claims
        claims.AddRange(_customClaims);

        // Create token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
