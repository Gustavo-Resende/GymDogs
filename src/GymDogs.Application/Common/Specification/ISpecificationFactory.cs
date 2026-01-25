using GymDogs.Domain.Exercises.Specification;
using GymDogs.Domain.ExerciseSets.Specification;
using GymDogs.Domain.FolderExercises.Specification;
using GymDogs.Domain.Profiles.Specification;
using GymDogs.Domain.Users.Specification;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.Common.Specification;

/// <summary>
/// Factory para criar Specifications de forma centralizada e normalizada.
/// Encapsula a lógica de criação e normalização de dados para as Specifications.
/// </summary>
public interface ISpecificationFactory
{
    // User Specifications
    GetUserByIdSpec CreateGetUserByIdSpec(Guid userId);
    GetUserByEmailSpec CreateGetUserByEmailSpec(string email);
    GetUserByUsernameSpec CreateGetUserByUsernameSpec(string username);
    
    // Profile Specifications
    GetProfileByIdSpec CreateGetProfileByIdSpec(Guid profileId);
    GetProfileByUserIdSpec CreateGetProfileByUserIdSpec(Guid userId);
    GetPublicProfilesSpec CreateGetPublicProfilesSpec();
    SearchPublicProfilesSpec CreateSearchPublicProfilesSpec(string searchTerm);
    
    // Exercise Specifications
    GetExerciseByIdSpec CreateGetExerciseByIdSpec(Guid exerciseId);
    SearchExercisesByNameSpec CreateSearchExercisesByNameSpec(string searchTerm);
    GetAvailableExercisesForFolderSpec CreateGetAvailableExercisesForFolderSpec(Guid workoutFolderId);
    SearchAvailableExercisesForFolderSpec CreateSearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm);
    
    // WorkoutFolder Specifications
    GetWorkoutFolderByIdSpec CreateGetWorkoutFolderByIdSpec(Guid workoutFolderId);
    GetWorkoutFoldersByProfileIdSpec CreateGetWorkoutFoldersByProfileIdSpec(Guid profileId);
    
    // FolderExercise Specifications
    GetFolderExerciseByIdSpec CreateGetFolderExerciseByIdSpec(Guid folderExerciseId);
    GetFolderExercisesByFolderIdSpec CreateGetFolderExercisesByFolderIdSpec(Guid folderId);
    GetFolderExerciseByFolderAndExerciseSpec CreateGetFolderExerciseByFolderAndExerciseSpec(
        Guid folderId, Guid exerciseId);
    
    // ExerciseSet Specifications
    GetExerciseSetByIdSpec CreateGetExerciseSetByIdSpec(Guid exerciseSetId);
    GetExerciseSetsByFolderExerciseIdSpec CreateGetExerciseSetsByFolderExerciseIdSpec(Guid folderExerciseId);
    
    // RefreshToken Specifications
    GetRefreshTokenByTokenSpec CreateGetRefreshTokenByTokenSpec(string token);
}
