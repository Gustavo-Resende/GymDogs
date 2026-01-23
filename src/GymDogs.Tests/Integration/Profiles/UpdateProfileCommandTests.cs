using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Commands;
using GymDogs.Domain.Profiles;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Profiles;

public class UpdateProfileCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateProfile()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Old Name", ProfileVisibility.Public);
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        profileRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(profile.Id, "New Name", "New Bio", userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.DisplayName);
        Assert.Equal("New Bio", result.Value.Bio);
        profileRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDifferentUserId_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profile = new Profile(userId, "Name", ProfileVisibility.Public);
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(profile.Id, "New Name", "New Bio", otherUserId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
        Assert.Contains("own profile", result.Errors.First(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_WithNullCurrentUserId_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Name", ProfileVisibility.Public);
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(profile.Id, "New Name", "New Bio", null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task Handle_WithNonExistentProfile_ShouldReturnNotFound()
    {
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync<Profile>(null);

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(Guid.NewGuid(), "New Name", "New Bio", Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(Guid.Empty, "New Name", "New Bio", Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithNullDisplayName_ShouldSetEmptyString()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Old Name", ProfileVisibility.Public);
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        profileRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(profile.Id, null, "Bio", userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Value.DisplayName);
    }

    [Fact]
    public async Task Handle_WithVeryLongBio_ShouldUpdate()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Name", ProfileVisibility.Public);
        var longBio = new string('a', 5000);
        var profileRepoMock = MockHelpers.CreateRepositoryMock<Profile>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        profileRepoMock.SetupFirstOrDefaultAsync(profile);
        profileRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Profile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateProfileCommandHandler(profileRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateProfileCommand(profile.Id, "Name", longBio, userId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(longBio, result.Value.Bio);
    }
}
