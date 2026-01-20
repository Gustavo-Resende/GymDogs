namespace GymDogs.Application.ExerciseSets.Dtos;

public record UpdateExerciseSetDto(
    int? Reps,
    decimal? Weight
);
