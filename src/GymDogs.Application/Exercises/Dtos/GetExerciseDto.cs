namespace GymDogs.Application.Exercises.Dtos;

public record GetExerciseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
);
