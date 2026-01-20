using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Domain.ExerciseSets;

namespace GymDogs.Application.ExerciseSets.Extensions;

public static class ExerciseSetExtensions
{
    public static CreateExerciseSetDto ToCreateExerciseSetDto(this ExerciseSet exerciseSet)
    {
        return new CreateExerciseSetDto(
            exerciseSet.Id,
            exerciseSet.FolderExerciseId,
            exerciseSet.SetNumber,
            exerciseSet.Reps,
            exerciseSet.Weight,
            exerciseSet.CreatedAt);
    }

    public static GetExerciseSetDto ToGetExerciseSetDto(this ExerciseSet exerciseSet)
    {
        return new GetExerciseSetDto(
            exerciseSet.Id,
            exerciseSet.FolderExerciseId,
            exerciseSet.SetNumber,
            exerciseSet.Reps,
            exerciseSet.Weight,
            exerciseSet.CreatedAt,
            exerciseSet.LastUpdatedAt);
    }
}
