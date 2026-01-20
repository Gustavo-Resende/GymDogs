namespace GymDogs.Application.FolderExercises.Dtos;

public record AddExerciseToFolderDto(
    Guid Id,
    Guid WorkoutFolderId,
    Guid ExerciseId,
    int Order,
    DateTime CreatedAt
);
