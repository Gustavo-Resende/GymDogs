using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Commands;
using GymDogs.Domain.Exercises;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.FolderExercises;

public class AddExerciseToFolderCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldAddExerciseToFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        folderExerciseRepoMock.SetupFirstOrDefaultAsync<FolderExercise>(null);
        folderExerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FolderExercise fe, CancellationToken ct) => fe);

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(folder.Id, exercise.Id, 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(folder.Id, result.Value.WorkoutFolderId);
        Assert.Equal(exercise.Id, result.Value.ExerciseId);
        folderExerciseRepoMock.Verify(x => x.AddAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentFolder_ShouldReturnNotFound()
    {
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync<WorkoutFolder>(null);

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(Guid.NewGuid(), Guid.NewGuid(), 0, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithNonExistentExercise_ShouldReturnNotFound()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        exerciseRepoMock.SetupFirstOrDefaultAsync<Exercise>(null);

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(folder.Id, Guid.NewGuid(), 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyWorkoutFolderId_ShouldReturnNotFound()
    {
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(Guid.Empty, Guid.NewGuid(), 0, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyExerciseId_ShouldReturnNotFound()
    {
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(Guid.NewGuid(), Guid.Empty, 0, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithExistingExerciseInFolder_ShouldReturnConflict()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var existingFolderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        folderExerciseRepoMock.SetupFirstOrDefaultAsync(existingFolderExercise);

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(folder.Id, exercise.Id, 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains("already added", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WithMaxIntOrder_ShouldAddExerciseToFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        folderExerciseRepoMock.SetupFirstOrDefaultAsync<FolderExercise>(null);
        folderExerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FolderExercise fe, CancellationToken ct) => fe);

        var handler = new AddExerciseToFolderCommandHandler(
            folderExerciseRepoMock.Object,
            folderRepoMock.Object,
            exerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseToFolderCommand(folder.Id, exercise.Id, int.MaxValue, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(int.MaxValue, result.Value.Order);
    }
}
