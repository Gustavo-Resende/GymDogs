using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Commands;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.WorkoutFolders;

public class UpdateWorkoutFolderOrderCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateOrder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 0);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderOrderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderOrderCommand(folder.Id, 5, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value.Order);
        folderRepoMock.Verify(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentFolder_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync<WorkoutFolder>(null);

        var handler = new UpdateWorkoutFolderOrderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderOrderCommand(Guid.NewGuid(), 5, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new UpdateWorkoutFolderOrderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderOrderCommand(Guid.Empty, 5, Guid.NewGuid());

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
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderOrderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderOrderCommand(folder.Id, int.MaxValue, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(int.MaxValue, result.Value.Order);
    }

    [Fact]
    public async Task Handle_WithZeroOrder_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Test Folder", null, 5);
        MockHelpers.SetupWorkoutFolderWithProfile(folder, profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderOrderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderOrderCommand(folder.Id, 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.Order);
    }
}
