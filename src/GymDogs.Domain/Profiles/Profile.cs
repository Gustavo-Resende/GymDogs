using Ardalis.GuardClauses;
using GymDogs.Domain.Users;
using GymDogs.Domain.WorkoutFolders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Domain.Profiles
{
    public class Profile : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string DisplayName { get; private set; }
        public string? Bio { get; private set; }
        public ProfileVisibility Visibility { get; private set; }

        // Navigation properties
        public User? User { get; private set; }
        public ICollection<WorkoutFolder> WorkoutFolders{ get; private set; } = new List<WorkoutFolder>();

        private Profile() { } //EF constructor

        public Profile(Guid userId, string? displayName = null, ProfileVisibility visibility = ProfileVisibility.Public)
        {
            UserId = Guard.Against.Default(userId, nameof(userId));
            DisplayName = displayName ?? string.Empty;
            Visibility = Guard.Against.EnumOutOfRange(visibility, nameof(visibility));
            
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string? displayName, string? bio)
        {
            DisplayName = displayName ?? string.Empty;
            Bio = bio;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdateVisibility(ProfileVisibility visibility)
        {
            Visibility = Guard.Against.EnumOutOfRange(visibility, nameof(visibility));
            LastUpdatedAt = DateTime.UtcNow;
        }

        public bool IsVisibleTo(Guid? requestingUserId)
        {
            if (Visibility == ProfileVisibility.Public)
            {
                return true;
            }
            return requestingUserId.HasValue && requestingUserId.Value == UserId;
        }
    }
}
