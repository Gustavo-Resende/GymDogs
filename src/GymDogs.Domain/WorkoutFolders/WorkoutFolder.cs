using Ardalis.GuardClauses;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.Profiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Domain.WorkoutFolders
{
    public class WorkoutFolder : BaseEntity
    {
        public Guid ProfileId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public int Order { get; private set; }

        // Navigation Properties
        public Profile Profile { get; private set; }
        public ICollection<FolderExercise> Exercises { get; private set; } = new List<FolderExercise>();

        private WorkoutFolder() { }

        public WorkoutFolder(Guid profileId, string name, string? description = null, int order = 0)
        {
            ProfileId = Guard.Against.Default(profileId, nameof(profileId));
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.StringTooLong(name, 200, nameof(name));
            Description = description;
            Order = Guard.Against.Negative(order, nameof(order));
            
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.StringTooLong(name, 200, nameof(name));
            Description = description;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrder(int order)
        {
            Order = Guard.Against.Negative(order, nameof(order));
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
