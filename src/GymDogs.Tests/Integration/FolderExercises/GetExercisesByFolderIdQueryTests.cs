using Ardalis.Result;
using Ardalis.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Queries;
using GymDogs.Domain.Exercises;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.Profiles;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.FolderExercises;

public class GetExercisesByFolderIdQueryTests
{
    [Fact]
    public async Task Handle_WithValidFolderId_ShouldReturnExercises()
    {
        var folderId = Guid.NewGuid();
        var exercise1 = new Exercise("Bench Press", "Chest exercise");
        var exercise2 = new Exercise("Squat", "Leg exercise");
        var folderExercise1 = new FolderExercise(folderId, exercise1.Id, 0);
        var folderExercise2 = new FolderExercise(folderId, exercise2.Id, 1);
        MockHelpers.SetupFolderExerciseWithExercise(folderExercise1, exercise1);
        MockHelpers.SetupFolderExerciseWithExercise(folderExercise2, exercise2);
        var folderExercises = new List<FolderExercise> { folderExercise1, folderExercise2 };

        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        folderExerciseRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<FolderExercise>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(folderExercises);

        var handler = new GetExercisesByFolderIdQueryHandler(folderExerciseRepoMock.Object);
        var query = new GetExercisesByFolderIdQuery(folderId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task Handle_WithEmptyList_ShouldReturnEmptyList()
    {
        var folderId = Guid.NewGuid();
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        folderExerciseRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<FolderExercise>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FolderExercise>());

        var handler = new GetExercisesByFolderIdQueryHandler(folderExerciseRepoMock.Object);
        var query = new GetExercisesByFolderIdQuery(folderId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();

        var handler = new GetExercisesByFolderIdQueryHandler(folderExerciseRepoMock.Object);
        var query = new GetExercisesByFolderIdQuery(Guid.Empty);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithManyExercises_ShouldReturnAll()
    {
        var folderId = Guid.NewGuid();
        var exercises = Enumerable.Range(1, 50)
            .Select(i => new Exercise($"Exercise {i}", $"Description {i}"))
            .ToList();
        var folderExercises = exercises
            .Select((e, i) =>
            {
                var fe = new FolderExercise(folderId, e.Id, i);
                MockHelpers.SetupFolderExerciseWithExercise(fe, e);
                return fe;
            })
            .ToList();

        var folderExerciseRepoMock = MockHelpers.CreateReadRepositoryMock<FolderExercise>();
        folderExerciseRepoMock.Setup(x => x.ListAsync(
            It.IsAny<ISpecification<FolderExercise>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(folderExercises);

        var handler = new GetExercisesByFolderIdQueryHandler(folderExerciseRepoMock.Object);
        var query = new GetExercisesByFolderIdQuery(folderId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(50, result.Value.Count());
    }
}
