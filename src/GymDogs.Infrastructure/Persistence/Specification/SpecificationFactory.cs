using GymDogs.Application.Common.Specification;
using GymDogs.Domain.Exercises.Specification;
using GymDogs.Domain.ExerciseSets.Specification;
using GymDogs.Domain.FolderExercises.Specification;
using GymDogs.Domain.Profiles.Specification;
using GymDogs.Domain.Users.Specification;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Infrastructure.Persistence.Specification;

/// <summary>
/// Factory implementation for creating Specifications.
/// Centralizes the logic for normalization (trim, toLowerInvariant, etc.) and creation.
/// </summary>
public class SpecificationFactory : ISpecificationFactory
{
    /// <summary>
    /// Creates a specification to get a user by ID.
    /// </summary>
    public GetUserByIdSpec CreateGetUserByIdSpec(Guid userId)
    {
        return new GetUserByIdSpec(userId);
    }

    /// <summary>
    /// Creates a specification to get a user by email.
    /// Centralized normalization: trim and lowercase.
    /// </summary>
    public GetUserByEmailSpec CreateGetUserByEmailSpec(string email)
    {
        // Centralized normalization: trim and lowercase
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        return new GetUserByEmailSpec(normalizedEmail);
    }

    /// <summary>
    /// Creates a specification to get a user by username.
    /// Centralized normalization: trim.
    /// </summary>
    public GetUserByUsernameSpec CreateGetUserByUsernameSpec(string username)
    {
        // Centralized normalization: trim
        var normalizedUsername = username?.Trim() ?? string.Empty;
        return new GetUserByUsernameSpec(normalizedUsername);
    }

    /// <summary>
    /// Creates a specification to get a profile by ID.
    /// </summary>
    public GetProfileByIdSpec CreateGetProfileByIdSpec(Guid profileId)
    {
        return new GetProfileByIdSpec(profileId);
    }

    /// <summary>
    /// Creates a specification to get a profile by user ID.
    /// </summary>
    public GetProfileByUserIdSpec CreateGetProfileByUserIdSpec(Guid userId)
    {
        return new GetProfileByUserIdSpec(userId);
    }

    /// <summary>
    /// Creates a specification to get all public profiles.
    /// </summary>
    public GetPublicProfilesSpec CreateGetPublicProfilesSpec()
    {
        return new GetPublicProfilesSpec();
    }

    /// <summary>
    /// Creates a specification to search public profiles by username or display name.
    /// Centralized normalization: trim.
    /// </summary>
    public SearchPublicProfilesSpec CreateSearchPublicProfilesSpec(string searchTerm)
    {
        // Centralized normalization: trim
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        return new SearchPublicProfilesSpec(normalizedSearchTerm);
    }

    /// <summary>
    /// Creates a specification to get an exercise by ID.
    /// </summary>
    public GetExerciseByIdSpec CreateGetExerciseByIdSpec(Guid exerciseId)
    {
        return new GetExerciseByIdSpec(exerciseId);
    }

    /// <summary>
    /// Creates a specification to search exercises by name (case-insensitive).
    /// Centralized normalization: trim.
    /// </summary>
    public SearchExercisesByNameSpec CreateSearchExercisesByNameSpec(string searchTerm)
    {
        // Centralized normalization: trim
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        return new SearchExercisesByNameSpec(normalizedSearchTerm);
    }

    /// <summary>
    /// Creates a specification to get exercises available (not added) for a workout folder.
    /// </summary>
    public GetAvailableExercisesForFolderSpec CreateGetAvailableExercisesForFolderSpec(Guid workoutFolderId)
    {
        return new GetAvailableExercisesForFolderSpec(workoutFolderId);
    }

    /// <summary>
    /// Creates a specification to search exercises available (not added) for a workout folder by name.
    /// Centralized normalization: trim.
    /// </summary>
    public SearchAvailableExercisesForFolderSpec CreateSearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm)
    {
        // Centralized normalization: trim
        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        return new SearchAvailableExercisesForFolderSpec(workoutFolderId, normalizedSearchTerm);
    }

    /// <summary>
    /// Creates a specification to get a workout folder by ID.
    /// </summary>
    public GetWorkoutFolderByIdSpec CreateGetWorkoutFolderByIdSpec(Guid workoutFolderId)
    {
        return new GetWorkoutFolderByIdSpec(workoutFolderId);
    }

    /// <summary>
    /// Creates a specification to get all workout folders for a profile.
    /// </summary>
    public GetWorkoutFoldersByProfileIdSpec CreateGetWorkoutFoldersByProfileIdSpec(Guid profileId)
    {
        return new GetWorkoutFoldersByProfileIdSpec(profileId);
    }

    /// <summary>
    /// Creates a specification to get a folder exercise by ID.
    /// </summary>
    public GetFolderExerciseByIdSpec CreateGetFolderExerciseByIdSpec(Guid folderExerciseId)
    {
        return new GetFolderExerciseByIdSpec(folderExerciseId);
    }

    /// <summary>
    /// Creates a specification to get all folder exercises for a workout folder.
    /// </summary>
    public GetFolderExercisesByFolderIdSpec CreateGetFolderExercisesByFolderIdSpec(Guid folderId)
    {
        return new GetFolderExercisesByFolderIdSpec(folderId);
    }

    /// <summary>
    /// Creates a specification to get a folder exercise by folder and exercise IDs.
    /// </summary>
    public GetFolderExerciseByFolderAndExerciseSpec CreateGetFolderExerciseByFolderAndExerciseSpec(
        Guid folderId, Guid exerciseId)
    {
        return new GetFolderExerciseByFolderAndExerciseSpec(folderId, exerciseId);
    }

    /// <summary>
    /// Creates a specification to get an exercise set by ID.
    /// </summary>
    public GetExerciseSetByIdSpec CreateGetExerciseSetByIdSpec(Guid exerciseSetId)
    {
        return new GetExerciseSetByIdSpec(exerciseSetId);
    }

    /// <summary>
    /// Creates a specification to get all exercise sets for a folder exercise.
    /// </summary>
    public GetExerciseSetsByFolderExerciseIdSpec CreateGetExerciseSetsByFolderExerciseIdSpec(Guid folderExerciseId)
    {
        return new GetExerciseSetsByFolderExerciseIdSpec(folderExerciseId);
    }

    /// <summary>
    /// Creates a specification to get a refresh token by token value.
    /// Centralized normalization: trim.
    /// </summary>
    public GetRefreshTokenByTokenSpec CreateGetRefreshTokenByTokenSpec(string token)
    {
        // Centralized normalization: trim
        var normalizedToken = token?.Trim() ?? string.Empty;
        return new GetRefreshTokenByTokenSpec(normalizedToken);
    }
}
