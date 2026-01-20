namespace GymDogs.Application.ExerciseSets.Dtos;

public record CreateExerciseSetDto(
    Guid Id,
    Guid FolderExerciseId,
    int SetNumber,
    int Reps,
    decimal Weight,
    DateTime CreatedAt
);
