namespace GymDogs.Application.WorkoutFolders.Dtos;

public record CreateWorkoutFolderDto(
    Guid Id,
    Guid ProfileId,
    string Name,
    string? Description,
    int Order,
    DateTime CreatedAt
);
