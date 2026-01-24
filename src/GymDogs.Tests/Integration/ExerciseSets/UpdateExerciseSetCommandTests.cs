using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Commands;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.ExerciseSets;

public class UpdateExerciseSetCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateExerciseSet()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var exerciseSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        MockHelpers.SetupExerciseSetWithFolderExercise(exerciseSet, folderExercise);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync(exerciseSet);
        setRepoMock.Setup(x => x.UpdateAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(exerciseSet.Id, 12, 55.5m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Value.Reps);
        Assert.Equal(55.5m, result.Value.Weight);
        setRepoMock.Verify(x => x.UpdateAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentSet_ShouldReturnNotFound()
    {
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync<ExerciseSet>(null);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(Guid.NewGuid(), 12, 55.5m, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(Guid.Empty, 12, 55.5m, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithNullRepsAndWeight_ShouldReturnInvalid()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var exerciseSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        MockHelpers.SetupExerciseSetWithFolderExercise(exerciseSet, folderExercise);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync(exerciseSet);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(exerciseSet.Id, null, null, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task Handle_WithOnlyReps_ShouldUpdateReps()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var exerciseSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        MockHelpers.SetupExerciseSetWithFolderExercise(exerciseSet, folderExercise);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync(exerciseSet);
        setRepoMock.Setup(x => x.UpdateAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(exerciseSet.Id, 12, null, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Value.Reps);
    }

    [Fact]
    public async Task Handle_WithOnlyWeight_ShouldUpdateWeight()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var exerciseSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        MockHelpers.SetupExerciseSetWithFolderExercise(exerciseSet, folderExercise);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync(exerciseSet);
        setRepoMock.Setup(x => x.UpdateAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(exerciseSet.Id, null, 55.5m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(55.5m, result.Value.Weight);
    }

    [Fact]
    public async Task Handle_WithMaxReps_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var exerciseSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        MockHelpers.SetupExerciseSetWithFolderExercise(exerciseSet, folderExercise);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync(exerciseSet);
        setRepoMock.Setup(x => x.UpdateAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(exerciseSet.Id, 1000, 50, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1000, result.Value.Reps);
    }

    [Fact]
    public async Task Handle_WithMaxWeight_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var exerciseSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        MockHelpers.SetupExerciseSetWithFolderExercise(exerciseSet, folderExercise);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        setRepoMock.SetupFirstOrDefaultAsync(exerciseSet);
        setRepoMock.Setup(x => x.UpdateAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseSetCommandHandler(setRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseSetCommand(exerciseSet.Id, 10, 10000, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(10000, result.Value.Weight);
    }
}
