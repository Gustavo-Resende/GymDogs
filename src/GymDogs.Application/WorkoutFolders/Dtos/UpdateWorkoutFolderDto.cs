namespace GymDogs.Application.WorkoutFolders.Dtos;

public record UpdateWorkoutFolderDto(
    string? Name,
    string? Description
);

public record UpdateWorkoutFolderOrderDto(
    int Order
);
