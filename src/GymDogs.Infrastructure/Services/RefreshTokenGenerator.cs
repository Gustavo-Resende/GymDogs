using GymDogs.Application.Interfaces;
using System.Security.Cryptography;

namespace GymDogs.Infrastructure.Services;

/// <summary>
/// Implementation of IRefreshTokenGenerator using cryptographically secure random bytes
/// </summary>
public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
}
