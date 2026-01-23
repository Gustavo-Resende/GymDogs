using Ardalis.Result;
using Ardalis.Specification;
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

public class AddExerciseSetCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldAddExerciseSet()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, 1, 10, 50.5m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.SetNumber);
        Assert.Equal(10, result.Value.Reps);
        Assert.Equal(50.5m, result.Value.Weight);
        setRepoMock.Verify(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullSetNumber_ShouldCalculateNextSetNumber()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var existingSet = new ExerciseSet(folderExercise.Id, 1, 10, 50);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet> { existingSet });
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, null, 10, 50.5m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.SetNumber);
    }

    [Fact]
    public async Task Handle_WithNullSetNumberAndNoExistingSets_ShouldUseSetNumberOne()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, null, 10, 50.5m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.SetNumber);
    }

    [Fact]
    public async Task Handle_WithNonExistentFolderExercise_ShouldReturnNotFound()
    {
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync<FolderExercise>(null);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(Guid.NewGuid(), 1, 10, 50, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyFolderExerciseId_ShouldReturnNotFound()
    {
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(Guid.Empty, 1, 10, 50, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithMaxReps_ShouldAddExerciseSet()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, 1, 1000, 50, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1000, result.Value.Reps);
    }

    [Fact]
    public async Task Handle_WithMaxWeight_ShouldAddExerciseSet()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, 1, 10, 10000, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(10000, result.Value.Weight);
    }

    [Fact]
    public async Task Handle_WithVerySmallWeight_ShouldAddExerciseSet()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, 1, 10, 0.0001m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0.0001m, result.Value.Weight);
    }

    [Fact]
    public async Task Handle_WithHighPrecisionWeight_ShouldAddExerciseSet()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Domain.Exercises.Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        var setRepoMock = MockHelpers.CreateRepositoryMock<ExerciseSet>();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());
        setRepoMock.Setup(x => x.AddAsync(It.IsAny<ExerciseSet>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExerciseSet s, CancellationToken ct) => s);

        var handler = new AddExerciseSetCommandHandler(
            setRepoMock.Object,
            folderExerciseRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new AddExerciseSetCommand(folderExercise.Id, 1, 10, 12.3456789m, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12.3456789m, result.Value.Weight);
    }
}
