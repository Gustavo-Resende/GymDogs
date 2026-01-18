using Ardalis.GuardClauses;
using GymDogs.Domain.Exercises;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.WorkoutFolders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Domain.FolderExercises
{
    public class FolderExercise : BaseEntity
    {
        public Guid WorkoutFolderId { get; private set; }
        public Guid ExerciseId { get; private set; }
        public int Order { get; private set; }

        // Navigation Properties
        public WorkoutFolder WorkoutFolder { get; private set; }
        public Exercise Exercise { get; private set; }
        public ICollection<ExerciseSet> Sets { get; private set; } = new List<ExerciseSet>();

        private FolderExercise() { } //EF constructor

        public FolderExercise(Guid workoutFolderId, Guid exerciseId, int order)
        {
            WorkoutFolderId = Guard.Against.Default(workoutFolderId, nameof(workoutFolderId));
            ExerciseId = Guard.Against.Default(exerciseId, nameof(exerciseId));
            Order = Guard.Against.Negative(order, nameof(order));
            
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOrder(int order)
        {
            Order = Guard.Against.Negative(order, nameof(order));
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
