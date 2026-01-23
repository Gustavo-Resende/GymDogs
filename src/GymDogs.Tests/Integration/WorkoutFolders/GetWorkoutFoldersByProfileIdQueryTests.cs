using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Queries;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.WorkoutFolders;

public class GetWorkoutFoldersByProfileIdQueryTests
{
    [Fact]
    public async Task Handle_WithValidProfileId_ShouldReturnFolders()
    {
        var profileId = Guid.NewGuid();
        var folders = new List<WorkoutFolder>
        {
            new WorkoutFolder(profileId, "Back Workout", "Back training", 1),
            new WorkoutFolder(profileId, "Chest Workout", "Chest training", 2),
            new WorkoutFolder(profileId, "Legs Workout", "Legs training", 0)
        };

        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        folderRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<WorkoutFolder>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(folders);

        var handler = new GetWorkoutFoldersByProfileIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFoldersByProfileIdQuery(profileId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count());
    }

    [Fact]
    public async Task Handle_WithEmptyList_ShouldReturnEmptyList()
    {
        var profileId = Guid.NewGuid();
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        folderRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<WorkoutFolder>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkoutFolder>());

        var handler = new GetWorkoutFoldersByProfileIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFoldersByProfileIdQuery(profileId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();

        var handler = new GetWorkoutFoldersByProfileIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFoldersByProfileIdQuery(Guid.Empty);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithManyFolders_ShouldReturnAll()
    {
        var profileId = Guid.NewGuid();
        var folders = Enumerable.Range(1, 100)
            .Select(i => new WorkoutFolder(profileId, $"Folder {i}", $"Description {i}", i))
            .ToList();

        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        folderRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<WorkoutFolder>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(folders);

        var handler = new GetWorkoutFoldersByProfileIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFoldersByProfileIdQuery(profileId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value.Count());
    }
}
