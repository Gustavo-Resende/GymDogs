using Ardalis.GuardClauses;
using GymDogs.Domain.FolderExercises;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Domain.Exercises
{
    public class Exercise : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }

        // Navigation Properties
        public ICollection<FolderExercise> FolderExercises { get; private set; } = new List<FolderExercise>();

        private Exercise() { } // EF constructor

        public Exercise(string name, string? description = null)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.StringTooLong(name, 200, nameof(name));
            
            if (!string.IsNullOrWhiteSpace(description))
            {
                Guard.Against.StringTooLong(description, 1000, nameof(description));
            }
            Description = description;
            
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description)
        {
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.StringTooLong(name, 200, nameof(name));
            
            if (!string.IsNullOrWhiteSpace(description))
            {
                Guard.Against.StringTooLong(description, 1000, nameof(description));
            }
            Description = description;
            
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
