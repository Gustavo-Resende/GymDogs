using Ardalis.Result;
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

public class UpdateFolderExerciseOrderCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateOrder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        MockHelpers.SetupFolderExerciseWithExercise(folderExercise, exercise);
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        folderExerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateFolderExerciseOrderCommandHandler(folderExerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateFolderExerciseOrderCommand(folderExercise.Id, 5, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.Order);
        folderExerciseRepoMock.Verify(x => x.UpdateAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentFolderExercise_ShouldReturnNotFound()
    {
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync<FolderExercise>(null);

        var handler = new UpdateFolderExerciseOrderCommandHandler(folderExerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateFolderExerciseOrderCommand(Guid.NewGuid(), 5, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new UpdateFolderExerciseOrderCommandHandler(folderExerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateFolderExerciseOrderCommand(Guid.Empty, 5, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithMaxIntOrder_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 0);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        MockHelpers.SetupFolderExerciseWithExercise(folderExercise, exercise);
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        folderExerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateFolderExerciseOrderCommandHandler(folderExerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateFolderExerciseOrderCommand(folderExercise.Id, int.MaxValue, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(int.MaxValue, result.Value.Order);
    }

    [Fact]
    public async Task Handle_WithZeroOrder_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var folderExercise = new FolderExercise(folder.Id, exercise.Id, 5);
        MockHelpers.SetupFolderExerciseWithWorkoutFolder(folderExercise, folder);
        MockHelpers.SetupFolderExerciseWithExercise(folderExercise, exercise);
        var folderExerciseRepoMock = MockHelpers.CreateRepositoryMock<FolderExercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderExerciseRepoMock.SetupFirstOrDefaultAsync(folderExercise);
        folderExerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<FolderExercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateFolderExerciseOrderCommandHandler(folderExerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateFolderExerciseOrderCommand(folderExercise.Id, 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.Order);
    }
}
