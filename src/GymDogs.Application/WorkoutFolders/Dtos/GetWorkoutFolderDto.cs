namespace GymDogs.Application.WorkoutFolders.Dtos;

public record GetWorkoutFolderDto(
    Guid Id,
    Guid ProfileId,
    string Name,
    string? Description,
    int Order,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
);
