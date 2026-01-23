using Ardalis.Result;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Queries;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.WorkoutFolders;

public class GetWorkoutFolderByIdQueryTests
{
    [Fact]
    public async Task Handle_WithValidId_ShouldReturnWorkoutFolder()
    {
        var profile = new Profile(Guid.NewGuid(), "Test User", ProfileVisibility.Public);
        var folder = new WorkoutFolder(profile.Id, "Back Workout", "Back training", 1);
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        folderRepoMock.SetupFirstOrDefaultAsync(folder);

        var handler = new GetWorkoutFolderByIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFolderByIdQuery(folder.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Back Workout", result.Value.Name);
        Assert.Equal("Back training", result.Value.Description);
        Assert.Equal(1, result.Value.Order);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();
        folderRepoMock.SetupFirstOrDefaultAsync<WorkoutFolder>(null);

        var handler = new GetWorkoutFolderByIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFolderByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var folderRepoMock = MockHelpers.CreateReadRepositoryMock<WorkoutFolder>();

        var handler = new GetWorkoutFolderByIdQueryHandler(folderRepoMock.Object);
        var query = new GetWorkoutFolderByIdQuery(Guid.Empty);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }
}
