namespace GymDogs.Application.ExerciseSets.Dtos;

public record GetExerciseSetDto(
    Guid Id,
    Guid FolderExerciseId,
    int SetNumber,
    int Reps,
    decimal Weight,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
);
