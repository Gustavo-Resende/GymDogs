using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Domain.Profiles;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;
using System.Reflection;

namespace GymDogs.Tests.Integration.Helpers;

public static class MockHelpers
{
    public static Mock<IReadRepository<T>> CreateReadRepositoryMock<T>() where T : class
    {
        return new Mock<IReadRepository<T>>();
    }

    public static Mock<IRepository<T>> CreateRepositoryMock<T>() where T : class
    {
        return new Mock<IRepository<T>>();
    }

    public static Mock<IUnitOfWork> CreateUnitOfWorkMock()
    {
        var mock = new Mock<IUnitOfWork>();
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        return mock;
    }

    public static Mock<IPasswordHasher> CreatePasswordHasherMock()
    {
        var mock = new Mock<IPasswordHasher>();
        mock.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns<string>(p => $"hashed_{p}");
        mock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((password, hash) => hash == $"hashed_{password}");
        return mock;
    }

    public static Mock<IJwtTokenGenerator> CreateJwtTokenGeneratorMock()
    {
        var mock = new Mock<IJwtTokenGenerator>();
        mock.Setup(x => x.GenerateToken(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int?>()))
            .Returns<Guid, string, string, string, int?>((id, username, email, role, exp) => 
                $"token_{id}_{username}_{email}_{role}");
        return mock;
    }

    public static Mock<IRefreshTokenGenerator> CreateRefreshTokenGeneratorMock()
    {
        var mock = new Mock<IRefreshTokenGenerator>();
        mock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh_token_12345");
        return mock;
    }

    public static Mock<IConfiguration> CreateConfigurationMock()
    {
        var mock = new Mock<IConfiguration>();
        var jwtSection = new Mock<IConfigurationSection>();
        jwtSection.Setup(x => x["AccessTokenExpirationMinutes"]).Returns("15");
        jwtSection.Setup(x => x["RefreshTokenExpirationDays"]).Returns("7");
        mock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);
        return mock;
    }

    public static void SetupFirstOrDefaultAsync<T>(
        this Mock<IReadRepository<T>> mock,
        T? returnValue) where T : class
    {
        mock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<T>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnValue);
    }

    public static void SetupFirstOrDefaultAsync<T>(
        this Mock<IRepository<T>> mock,
        T? returnValue) where T : class
    {
        mock.Setup(x => x.FirstOrDefaultAsync(
            It.IsAny<ISpecification<T>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnValue);
    }

    public static void SetupListAsync<T>(
        this Mock<IReadRepository<T>> mock,
        IEnumerable<T> returnValue) where T : class
    {
        mock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<T>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnValue.ToList());
    }

    public static void SetNavigationProperty<TEntity, TProperty>(
        TEntity entity,
        string propertyName,
        TProperty value) where TEntity : class
    {
        var property = typeof(TEntity).GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        
        if (property != null)
        {
            var backingField = typeof(TEntity).GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (backingField != null)
            {
                backingField.SetValue(entity, value);
            }
            else if (property.SetMethod != null)
            {
                property.SetValue(entity, value);
            }
        }
    }

    public static void SetupWorkoutFolderWithProfile(WorkoutFolder folder, Profile profile)
    {
        SetNavigationProperty(folder, "Profile", profile);
    }

    public static void SetupFolderExerciseWithWorkoutFolder(FolderExercise folderExercise, WorkoutFolder workoutFolder)
    {
        SetNavigationProperty(folderExercise, "WorkoutFolder", workoutFolder);
    }

    public static void SetupFolderExerciseWithExercise(FolderExercise folderExercise, Domain.Exercises.Exercise exercise)
    {
        SetNavigationProperty(folderExercise, "Exercise", exercise);
    }

    public static void SetupExerciseSetWithFolderExercise(ExerciseSet exerciseSet, FolderExercise folderExercise)
    {
        SetNavigationProperty(exerciseSet, "FolderExercise", folderExercise);
    }
}
