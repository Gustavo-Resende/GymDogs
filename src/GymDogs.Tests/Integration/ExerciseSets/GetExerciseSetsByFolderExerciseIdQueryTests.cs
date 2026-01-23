using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Queries;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.ExerciseSets;

public class GetExerciseSetsByFolderExerciseIdQueryTests
{
    [Fact]
    public async Task Handle_WithValidFolderExerciseId_ShouldReturnExerciseSets()
    {
        var folderExerciseId = Guid.NewGuid();
        var exerciseSets = new List<ExerciseSet>
        {
            new ExerciseSet(folderExerciseId, 1, 10, 50),
            new ExerciseSet(folderExerciseId, 2, 12, 55),
            new ExerciseSet(folderExerciseId, 3, 8, 60)
        };

        var setRepoMock = MockHelpers.CreateReadRepositoryMock<ExerciseSet>();
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseSets);

        var handler = new GetExerciseSetsByFolderExerciseIdQueryHandler(setRepoMock.Object);
        var query = new GetExerciseSetsByFolderExerciseIdQuery(folderExerciseId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(3, result.Value.Count());
    }

    [Fact]
    public async Task Handle_WithEmptyList_ShouldReturnEmptyList()
    {
        var folderExerciseId = Guid.NewGuid();
        var setRepoMock = MockHelpers.CreateReadRepositoryMock<ExerciseSet>();
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ExerciseSet>());

        var handler = new GetExerciseSetsByFolderExerciseIdQueryHandler(setRepoMock.Object);
        var query = new GetExerciseSetsByFolderExerciseIdQuery(folderExerciseId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var setRepoMock = MockHelpers.CreateReadRepositoryMock<ExerciseSet>();

        var handler = new GetExerciseSetsByFolderExerciseIdQueryHandler(setRepoMock.Object);
        var query = new GetExerciseSetsByFolderExerciseIdQuery(Guid.Empty);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithManySets_ShouldReturnAll()
    {
        var folderExerciseId = Guid.NewGuid();
        var exerciseSets = Enumerable.Range(1, 50)
            .Select(i => new ExerciseSet(folderExerciseId, i, 10, 50))
            .ToList();

        var setRepoMock = MockHelpers.CreateReadRepositoryMock<ExerciseSet>();
        setRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<ExerciseSet>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(exerciseSets);

        var handler = new GetExerciseSetsByFolderExerciseIdQueryHandler(setRepoMock.Object);
        var query = new GetExerciseSetsByFolderExerciseIdQuery(folderExerciseId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(50, result.Value.Count());
    }
}
