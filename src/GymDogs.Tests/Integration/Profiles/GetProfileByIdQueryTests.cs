using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Profiles.Queries;
using GymDogs.Domain.Profiles;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Profiles;

public class GetProfileByIdQueryTests
{
    [Fact]
    public async Task Handle_WithPublicProfile_ShouldReturnProfile()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(profile.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test User", result.Value.DisplayName);
    }

    [Fact]
    public async Task Handle_WithPrivateProfileAndOwner_ShouldReturnProfile()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Private);
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(profile.Id, userId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Handle_WithPrivateProfileAndOtherUser_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Private);
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(profile.Id, otherUserId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task Handle_WithPrivateProfileAndNullUserId_ShouldReturnForbidden()
    {
        var userId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Private);
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(profile.Id, null);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task Handle_WithNonExistentProfile_ShouldReturnNotFound()
    {
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupFirstOrDefaultAsync<Profile>(null);

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(Guid.Empty);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithPublicProfileAndAnyUser_ShouldReturnProfile()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profile = new Profile(userId, "Test User", ProfileVisibility.Public);
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupFirstOrDefaultAsync(profile);

        var handler = new GetProfileByIdQueryHandler(profileRepoMock.Object);
        var query = new GetProfileByIdQuery(profile.Id, otherUserId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
