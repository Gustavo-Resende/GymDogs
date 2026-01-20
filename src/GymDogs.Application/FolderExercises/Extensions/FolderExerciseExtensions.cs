using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Domain.FolderExercises;

namespace GymDogs.Application.FolderExercises.Extensions;

public static class FolderExerciseExtensions
{
    public static AddExerciseToFolderDto ToAddExerciseToFolderDto(this FolderExercise folderExercise)
    {
        return new AddExerciseToFolderDto(
            folderExercise.Id,
            folderExercise.WorkoutFolderId,
            folderExercise.ExerciseId,
            folderExercise.Order,
            folderExercise.CreatedAt);
    }

    public static GetFolderExerciseDto ToGetFolderExerciseDto(this FolderExercise folderExercise)
    {
        return new GetFolderExerciseDto(
            folderExercise.Id,
            folderExercise.WorkoutFolderId,
            folderExercise.ExerciseId,
            folderExercise.Exercise.Name,
            folderExercise.Exercise.Description,
            folderExercise.Order,
            folderExercise.CreatedAt,
            folderExercise.LastUpdatedAt);
    }
}
