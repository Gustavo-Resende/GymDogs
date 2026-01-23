using Ardalis.GuardClauses;
using GymDogs.Domain.Profiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Domain.Users
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Role Role { get; private set; } = Role.User;
        
        //Navigation properties
        public Profile Profile { get; private set; }

        private User() { } //EF constructor

        public User(string username, string email, string passwordHash, Role role = Role.User)
        {
            Username = Guard.Against.NullOrWhiteSpace(username, nameof(username));
            Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
            PasswordHash = Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash));
            Role = Guard.Against.Null(role, nameof(role));

            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdateUsername(string username)
        {
            Username = Guard.Against.NullOrWhiteSpace(username, nameof(username));
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void SetRole(Role role)
        {
            Role = Guard.Against.Null(role, nameof(role));
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
