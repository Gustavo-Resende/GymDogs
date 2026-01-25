using GymDogs.Application.Common.Specification;
using GymDogs.Domain.Exercises.Specification;
using GymDogs.Domain.ExerciseSets.Specification;
using GymDogs.Domain.FolderExercises.Specification;
using GymDogs.Domain.Profiles.Specification;
using GymDogs.Domain.Users.Specification;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Infrastructure.Persistence.Specification;

/// <summary>
/// Implementação do Factory para criar Specifications.
/// Centraliza a lógica de normalização (trim, toLowerInvariant, etc.) e criação.
/// </summary>
public class SpecificationFactory : ISpecificationFactory
{
    // User Specifications
    public GetUserByIdSpec CreateGetUserByIdSpec(Guid userId)
    {
        return new GetUserByIdSpec(userId);
    }

    public GetUserByEmailSpec CreateGetUserByEmailSpec(string email)
    {
        // Normalização centralizada: trim e lowercase
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        return new GetUserByEmailSpec(normalizedEmail);
    }

    public GetUserByUsernameSpec CreateGetUserByUsernameSpec(string username)
    {
        // Normalização centralizada: trim
        var normalizedUsername = username?.Trim() ?? string.Empty;
        return new GetUserByUsernameSpec(normalizedUsername);
    }

    // Profile Specifications
    public GetProfileByIdSpec CreateGetProfileByIdSpec(Guid profileId)
    {
        return new GetProfileByIdSpec(profileId);
    }

    public GetProfileByUserIdSpec CreateGetProfileByUserIdSpec(Guid userId)
    {
        return new GetProfileByUserIdSpec(userId);
    }

    public GetPublicProfilesSpec CreateGetPublicProfilesSpec()
    {
        return new GetPublicProfilesSpec();
    }

    public SearchPublicProfilesSpec CreateSearchPublicProfilesSpec(string searchTerm)
    {
        // Normalização centralizada: trim
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        return new SearchPublicProfilesSpec(normalizedSearchTerm);
    }

    // Exercise Specifications
    public GetExerciseByIdSpec CreateGetExerciseByIdSpec(Guid exerciseId)
    {
        return new GetExerciseByIdSpec(exerciseId);
    }

    public SearchExercisesByNameSpec CreateSearchExercisesByNameSpec(string searchTerm)
    {
        // Normalização centralizada: trim
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        return new SearchExercisesByNameSpec(normalizedSearchTerm);
    }

    public GetAvailableExercisesForFolderSpec CreateGetAvailableExercisesForFolderSpec(Guid workoutFolderId)
    {
        return new GetAvailableExercisesForFolderSpec(workoutFolderId);
    }

    public SearchAvailableExercisesForFolderSpec CreateSearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm)
    {
        // Normalização centralizada: trim
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        return new SearchAvailableExercisesForFolderSpec(workoutFolderId, normalizedSearchTerm);
    }

    // WorkoutFolder Specifications
    public GetWorkoutFolderByIdSpec CreateGetWorkoutFolderByIdSpec(Guid workoutFolderId)
    {
        return new GetWorkoutFolderByIdSpec(workoutFolderId);
    }

    public GetWorkoutFoldersByProfileIdSpec CreateGetWorkoutFoldersByProfileIdSpec(Guid profileId)
    {
        return new GetWorkoutFoldersByProfileIdSpec(profileId);
    }

    // FolderExercise Specifications
    public GetFolderExerciseByIdSpec CreateGetFolderExerciseByIdSpec(Guid folderExerciseId)
    {
        return new GetFolderExerciseByIdSpec(folderExerciseId);
    }

    public GetFolderExercisesByFolderIdSpec CreateGetFolderExercisesByFolderIdSpec(Guid folderId)
    {
        return new GetFolderExercisesByFolderIdSpec(folderId);
    }

    public GetFolderExerciseByFolderAndExerciseSpec CreateGetFolderExerciseByFolderAndExerciseSpec(
        Guid folderId, Guid exerciseId)
    {
        return new GetFolderExerciseByFolderAndExerciseSpec(folderId, exerciseId);
    }

    // ExerciseSet Specifications
    public GetExerciseSetByIdSpec CreateGetExerciseSetByIdSpec(Guid exerciseSetId)
    {
        return new GetExerciseSetByIdSpec(exerciseSetId);
    }

    public GetExerciseSetsByFolderExerciseIdSpec CreateGetExerciseSetsByFolderExerciseIdSpec(Guid folderExerciseId)
    {
        return new GetExerciseSetsByFolderExerciseIdSpec(folderExerciseId);
    }

    // RefreshToken Specifications
    public GetRefreshTokenByTokenSpec CreateGetRefreshTokenByTokenSpec(string token)
    {
        // Normalização centralizada: trim
        var normalizedToken = token?.Trim() ?? string.Empty;
        return new GetRefreshTokenByTokenSpec(normalizedToken);
    }
}
