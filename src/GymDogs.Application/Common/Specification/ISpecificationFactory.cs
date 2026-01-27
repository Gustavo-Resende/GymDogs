using GymDogs.Domain.Exercises.Specification;
using GymDogs.Domain.ExerciseSets.Specification;
using GymDogs.Domain.FolderExercises.Specification;
using GymDogs.Domain.Profiles.Specification;
using GymDogs.Domain.Users.Specification;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.Common.Specification;

/// <summary>
/// Factory interface for creating Specifications in a centralized and normalized way.
/// Encapsulates the logic for creating and normalizing data for Specifications.
/// </summary>
public interface ISpecificationFactory
{
    /// <summary>
    /// Creates a specification to get a user by ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>A specification to query a user by ID</returns>
    GetUserByIdSpec CreateGetUserByIdSpec(Guid userId);
    
    /// <summary>
    /// Creates a specification to get a user by email.
    /// The email will be normalized (trimmed and lowercased) before creating the specification.
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <returns>A specification to query a user by email</returns>
    GetUserByEmailSpec CreateGetUserByEmailSpec(string email);
    
    /// <summary>
    /// Creates a specification to get a user by username.
    /// The username will be normalized (trimmed) before creating the specification.
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <returns>A specification to query a user by username</returns>
    GetUserByUsernameSpec CreateGetUserByUsernameSpec(string username);
    
    /// <summary>
    /// Creates a specification to get a profile by ID.
    /// </summary>
    /// <param name="profileId">The unique identifier of the profile</param>
    /// <returns>A specification to query a profile by ID</returns>
    GetProfileByIdSpec CreateGetProfileByIdSpec(Guid profileId);
    
    /// <summary>
    /// Creates a specification to get a profile by user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>A specification to query a profile by user ID</returns>
    GetProfileByUserIdSpec CreateGetProfileByUserIdSpec(Guid userId);
    
    /// <summary>
    /// Creates a specification to get all public profiles.
    /// </summary>
    /// <returns>A specification to query all public profiles</returns>
    GetPublicProfilesSpec CreateGetPublicProfilesSpec();
    
    /// <summary>
    /// Creates a specification to search public profiles by username or display name.
    /// The search term will be normalized (trimmed) before creating the specification.
    /// </summary>
    /// <param name="searchTerm">The search term to filter profiles</param>
    /// <returns>A specification to search public profiles</returns>
    SearchPublicProfilesSpec CreateSearchPublicProfilesSpec(string searchTerm);
    
    /// <summary>
    /// Creates a specification to get an exercise by ID.
    /// </summary>
    /// <param name="exerciseId">The unique identifier of the exercise</param>
    /// <returns>A specification to query an exercise by ID</returns>
    GetExerciseByIdSpec CreateGetExerciseByIdSpec(Guid exerciseId);
    
    /// <summary>
    /// Creates a specification to search exercises by name (case-insensitive).
    /// The search term will be normalized (trimmed) before creating the specification.
    /// </summary>
    /// <param name="searchTerm">The search term to filter exercises by name</param>
    /// <returns>A specification to search exercises by name</returns>
    SearchExercisesByNameSpec CreateSearchExercisesByNameSpec(string searchTerm);
    
    /// <summary>
    /// Creates a specification to get exercises available (not added) for a workout folder.
    /// </summary>
    /// <param name="workoutFolderId">The unique identifier of the workout folder</param>
    /// <returns>A specification to query available exercises for a folder</returns>
    GetAvailableExercisesForFolderSpec CreateGetAvailableExercisesForFolderSpec(Guid workoutFolderId);
    
    /// <summary>
    /// Creates a specification to search exercises available (not added) for a workout folder by name.
    /// The search term will be normalized (trimmed) before creating the specification.
    /// </summary>
    /// <param name="workoutFolderId">The unique identifier of the workout folder</param>
    /// <param name="searchTerm">The search term to filter exercises by name</param>
    /// <returns>A specification to search available exercises for a folder</returns>
    SearchAvailableExercisesForFolderSpec CreateSearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm);
    
    /// <summary>
    /// Creates a specification to get a workout folder by ID.
    /// </summary>
    /// <param name="workoutFolderId">The unique identifier of the workout folder</param>
    /// <returns>A specification to query a workout folder by ID</returns>
    GetWorkoutFolderByIdSpec CreateGetWorkoutFolderByIdSpec(Guid workoutFolderId);
    
    /// <summary>
    /// Creates a specification to get all workout folders for a profile.
    /// </summary>
    /// <param name="profileId">The unique identifier of the profile</param>
    /// <returns>A specification to query workout folders by profile ID</returns>
    GetWorkoutFoldersByProfileIdSpec CreateGetWorkoutFoldersByProfileIdSpec(Guid profileId);
    
    /// <summary>
    /// Creates a specification to get a folder exercise by ID.
    /// </summary>
    /// <param name="folderExerciseId">The unique identifier of the folder exercise</param>
    /// <returns>A specification to query a folder exercise by ID</returns>
    GetFolderExerciseByIdSpec CreateGetFolderExerciseByIdSpec(Guid folderExerciseId);
    
    /// <summary>
    /// Creates a specification to get all folder exercises for a workout folder.
    /// </summary>
    /// <param name="folderId">The unique identifier of the workout folder</param>
    /// <returns>A specification to query folder exercises by folder ID</returns>
    GetFolderExercisesByFolderIdSpec CreateGetFolderExercisesByFolderIdSpec(Guid folderId);
    
    /// <summary>
    /// Creates a specification to get a folder exercise by folder and exercise IDs.
    /// </summary>
    /// <param name="folderId">The unique identifier of the workout folder</param>
    /// <param name="exerciseId">The unique identifier of the exercise</param>
    /// <returns>A specification to query a folder exercise by folder and exercise IDs</returns>
    GetFolderExerciseByFolderAndExerciseSpec CreateGetFolderExerciseByFolderAndExerciseSpec(
        Guid folderId, Guid exerciseId);
    
    /// <summary>
    /// Creates a specification to get an exercise set by ID.
    /// </summary>
    /// <param name="exerciseSetId">The unique identifier of the exercise set</param>
    /// <returns>A specification to query an exercise set by ID</returns>
    GetExerciseSetByIdSpec CreateGetExerciseSetByIdSpec(Guid exerciseSetId);
    
    /// <summary>
    /// Creates a specification to get all exercise sets for a folder exercise.
    /// </summary>
    /// <param name="folderExerciseId">The unique identifier of the folder exercise</param>
    /// <returns>A specification to query exercise sets by folder exercise ID</returns>
    GetExerciseSetsByFolderExerciseIdSpec CreateGetExerciseSetsByFolderExerciseIdSpec(Guid folderExerciseId);
    
    /// <summary>
    /// Creates a specification to get a refresh token by token value.
    /// The token will be normalized (trimmed) before creating the specification.
    /// </summary>
    /// <param name="token">The refresh token value</param>
    /// <returns>A specification to query a refresh token by token value</returns>
    GetRefreshTokenByTokenSpec CreateGetRefreshTokenByTokenSpec(string token);
}
