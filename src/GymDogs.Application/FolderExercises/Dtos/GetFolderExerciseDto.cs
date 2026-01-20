namespace GymDogs.Application.FolderExercises.Dtos;

public record GetFolderExerciseDto(
    Guid Id,
    Guid WorkoutFolderId,
    Guid ExerciseId,
    string ExerciseName,
    string? ExerciseDescription,
    int Order,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
);
