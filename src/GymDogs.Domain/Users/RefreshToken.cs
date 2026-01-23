using Ardalis.GuardClauses;
using System;

namespace GymDogs.Domain.Users;

/// <summary>
/// Refresh token entity for JWT token renewal
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    // Navigation property
    public User? User { get; private set; }

    private RefreshToken() { } // EF constructor

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = Guard.Against.Default(userId, nameof(userId));
        Token = Guard.Against.NullOrWhiteSpace(token, nameof(token));
        ExpiresAt = Guard.Against.Default(expiresAt, nameof(expiresAt));

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("ExpiresAt must be in the future.", nameof(expiresAt));
        }

        IsRevoked = false;
        RevokedAt = null;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        if (IsRevoked)
        {
            return;
        }

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAt;
    }

    public bool IsValid()
    {
        return !IsRevoked && !IsExpired();
    }
}
