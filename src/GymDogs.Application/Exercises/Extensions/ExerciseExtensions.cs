using GymDogs.Application.Exercises.Dtos;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Extensions;

public static class ExerciseExtensions
{
    public static CreateExerciseDto ToCreateExerciseDto(this Exercise exercise)
    {
        return new CreateExerciseDto(
            exercise.Id,
            exercise.Name,
            exercise.Description,
            exercise.CreatedAt);
    }

    public static GetExerciseDto ToGetExerciseDto(this Exercise exercise)
    {
        return new GetExerciseDto(
            exercise.Id,
            exercise.Name,
            exercise.Description,
            exercise.CreatedAt,
            exercise.LastUpdatedAt);
    }
}
