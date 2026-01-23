using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Commands;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.WorkoutFolders;

public class CreateWorkoutFolderCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateWorkoutFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        folderRepoMock.Setup(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutFolder f, CancellationToken ct) => f);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "Back Workout", "Back training", 1, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Back Workout", result.Value.Name);
        Assert.Equal("Back training", result.Value.Description);
        Assert.Equal(1, result.Value.Order);
        folderRepoMock.Verify(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentProfile_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync<Profile>(null);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(Guid.NewGuid(), "Test", null, 0, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyProfileId_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(Guid.Empty, "Test", null, 0, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithDifferentUserId_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "Test", null, 0, otherUserId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
        Assert.Contains("own profile", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WithNullCurrentUserId_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "Test", null, 0, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task Handle_WithNameWithWhitespace_ShouldNormalizeName()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        folderRepoMock.Setup(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutFolder f, CancellationToken ct) => f);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "  Back Workout  ", "Description", 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Back Workout", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithDescriptionWithWhitespace_ShouldNormalizeDescription()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        folderRepoMock.Setup(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutFolder f, CancellationToken ct) => f);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "Test", "  Description  ", 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Description", result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithMaxLengthName_ShouldCreateWorkoutFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var maxName = new string('a', 200);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        folderRepoMock.Setup(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutFolder f, CancellationToken ct) => f);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, maxName, "Description", 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(maxName, result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithVeryLongDescription_ShouldCreateWorkoutFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var longDescription = new string('a', 5000);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        folderRepoMock.Setup(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutFolder f, CancellationToken ct) => f);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "Test", longDescription, 0, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(longDescription, result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithMaxIntOrder_ShouldCreateWorkoutFolder()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        folderRepoMock.Setup(x => x.AddAsync(It.IsAny<WorkoutFolder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutFolder f, CancellationToken ct) => f);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, "Test", null, int.MaxValue, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(int.MaxValue, result.Value.Order);
    }

    [Fact]
    public async Task Handle_WithNullName_ShouldThrowException()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var folderRepoMock = MockHelpers.CreateRepositoryMock<WorkoutFolder>();
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new CreateWorkoutFolderCommandHandler(
            folderRepoMock.Object,
            profileRepoMock.Object,
            unitOfWorkMock.Object);

        var command = new CreateWorkoutFolderCommand(profile.Id, null!, "Description", 0, userId);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }
}
