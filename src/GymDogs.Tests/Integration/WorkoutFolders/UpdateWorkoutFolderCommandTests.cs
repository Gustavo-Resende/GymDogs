using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Commands;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.WorkoutFolders;

public class UpdateWorkoutFolderCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateWorkoutFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Old Name", "Old Description", 0);
        MockHelpers.SetNavigationProperty(folder, "Profile", profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(folder.Id, "New Name", "New Description", userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
        Assert.Equal("New Description", result.Value.Description);
        folderRepoMock.Verify(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentFolder_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync<WorkoutFolder>(null);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(Guid.NewGuid(), "New Name", "New Description", Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(Guid.Empty, "New Name", "New Description", Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithNullNameAndDescription_ShouldReturnInvalid()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Old Name", "Old Description", 0);
        MockHelpers.SetNavigationProperty(folder, "Profile", profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(folder.Id, null, null, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task Handle_WithOnlyName_ShouldUpdateName()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Old Name", "Old Description", 0);
        MockHelpers.SetNavigationProperty(folder, "Profile", profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(folder.Id, "New Name", null, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithOnlyDescription_ShouldUpdateDescription()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Old Name", "Old Description", 0);
        MockHelpers.SetNavigationProperty(folder, "Profile", profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(folder.Id, null, "New Description", userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Description", result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithNameWithWhitespace_ShouldNormalizeName()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Old Name", "Old Description", 0);
        MockHelpers.SetNavigationProperty(folder, "Profile", profile);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(folder.Id, "  New Name  ", "Description", userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithMaxLengthName_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Old Name", "Old Description", 0);
        MockHelpers.SetNavigationProperty(folder, "Profile", profile);
        var maxName = new string('a', 200);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        folderRepoMock.SetupFirstOrDefaultAsync(folder);
        folderRepoMock.Setup(x => x.UpdateAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateWorkoutFolderCommandHandler(folderRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateWorkoutFolderCommand(folder.Id, maxName, "Description", userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(maxName, result.Value.Name);
    }
}

