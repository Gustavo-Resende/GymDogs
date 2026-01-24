using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Profiles.Dtos;
using GymDogs.Application.Profiles.Queries;
using GymDogs.Domain.Profiles;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Profiles;

public class SearchPublicProfilesQueryTests
{
    [Fact]
    public async Task Handle_WithValidSearchTerm_ShouldReturnMatchingPublicProfiles()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var profile1 = new Profile(userId1, "João Silva", ProfileVisibility.Public);
        var profile2 = new Profile(userId2, "Maria Santos", ProfileVisibility.Public);

        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupListAsync(new[] { profile1 });

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("joao");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var profiles = result.Value.ToList();
        Assert.Single(profiles);
        Assert.Equal(profile1.Id, profiles.First().Id);
        Assert.Equal("João Silva", profiles.First().DisplayName);
    }

    [Fact]
    public async Task Handle_WithSearchTermMatchingDisplayName_ShouldReturnMatchingPublicProfiles()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var profile1 = new Profile(userId1, "João Silva", ProfileVisibility.Public);
        var profile2 = new Profile(userId2, "Maria Santos", ProfileVisibility.Public);

        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupListAsync(new[] { profile1 });

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("Silva");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var profiles = result.Value.ToList();
        Assert.Single(profiles);
        Assert.Equal("João Silva", profiles.First().DisplayName);
    }

    [Fact]
    public async Task Handle_WithEmptySearchTerm_ShouldReturnInvalid()
    {
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        profileRepoMock.Verify(x => x.ListAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithWhitespaceSearchTerm_ShouldReturnInvalid()
    {
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("   ");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
        profileRepoMock.Verify(x => x.ListAsync(It.IsAny<ISpecification<Profile>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullSearchTerm_ShouldReturnInvalid()
    {
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery(null!);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task Handle_WithNoMatchingProfiles_ShouldReturnEmptyList()
    {
        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupListAsync(Array.Empty<Profile>());

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("nonexistent");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldOnlyReturnPublicProfiles()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var profile1 = new Profile(userId1, "João Silva", ProfileVisibility.Public);
        var profile2 = new Profile(userId2, "Maria Santos", ProfileVisibility.Private);

        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupListAsync(new[] { profile1 });

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("joao");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var profiles = result.Value.ToList();
        Assert.Single(profiles);
        Assert.Equal(ProfileVisibilityDto.Public, profiles.First().Visibility);
    }

    [Fact]
    public async Task Handle_WithMultipleMatchingProfiles_ShouldReturnAllMatches()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var profile1 = new Profile(userId1, "João Silva", ProfileVisibility.Public);
        var profile2 = new Profile(userId2, "João Santos", ProfileVisibility.Public);

        var profileRepoMock = MockHelpers.CreateReadRepositoryMock<Profile>();
        profileRepoMock.SetupListAsync(new[] { profile1, profile2 });

        var handler = new SearchPublicProfilesQueryHandler(profileRepoMock.Object);

        var query = new SearchPublicProfilesQuery("João");

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var profiles = result.Value.ToList();
        Assert.Equal(2, profiles.Count);
    }
}
