namespace GymDogs.Application.Exercises.Dtos;

public record CreateExerciseDto(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt
);
