using Ardalis.GuardClauses;
using GymDogs.Domain.FolderExercises;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Domain.ExerciseSets
{
    public class ExerciseSet : BaseEntity
    {
        public Guid FolderExerciseId { get; private set; }
        public int SetNumber { get; private set; }
        public int Reps { get; private set; }
        public decimal Weight { get; private set; }

        // Navigation Properties
        public FolderExercise FolderExercise { get; private set; }

        private ExerciseSet() { } // EF constructor

        public ExerciseSet(Guid folderExerciseId, int setNumber, int reps, decimal weight)
        {
            FolderExerciseId = Guard.Against.Default(folderExerciseId, nameof(folderExerciseId));
            SetNumber = Guard.Against.NegativeOrZero(setNumber, nameof(setNumber));
            Reps = Guard.Against.NegativeOrZero(reps, nameof(reps));
            Guard.Against.OutOfRange(reps, nameof(reps), 1, 1000);
            Weight = Guard.Against.Negative(weight, nameof(weight));
            Guard.Against.OutOfRange(weight, nameof(weight), 0m, 10000m); // Max 10 toneladas
            
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }

        public void Update(int reps, decimal weight)
        {
            Reps = Guard.Against.NegativeOrZero(reps, nameof(reps));
            Guard.Against.OutOfRange(reps, nameof(reps), 1, 1000);
            Weight = Guard.Against.Negative(weight, nameof(weight));
            Guard.Against.OutOfRange(weight, nameof(weight), 0m, 10000m);
            
            LastUpdatedAt = DateTime.UtcNow;
        }
    }
}
