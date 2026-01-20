using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Domain.WorkoutFolders;

namespace GymDogs.Application.WorkoutFolders.Extensions;

public static class WorkoutFolderExtensions
{
    public static CreateWorkoutFolderDto ToCreateWorkoutFolderDto(this WorkoutFolder folder)
    {
        return new CreateWorkoutFolderDto(
            folder.Id,
            folder.ProfileId,
            folder.Name,
            folder.Description,
            folder.Order,
            folder.CreatedAt);
    }

    public static GetWorkoutFolderDto ToGetWorkoutFolderDto(this WorkoutFolder folder)
    {
        return new GetWorkoutFolderDto(
            folder.Id,
            folder.ProfileId,
            folder.Name,
            folder.Description,
            folder.Order,
            folder.CreatedAt,
            folder.LastUpdatedAt);
    }
}
