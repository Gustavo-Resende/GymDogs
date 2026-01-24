using Ardalis.Result;
using GymDogs.Application.Exercises.Queries;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.Exercises;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Exercises;

public class GetAllExercisesQueryTests
{
    [Fact]
    public async Task Handle_WithExercises_ShouldReturnOrderedList()
    {
        var exercises = new List<Exercise>
        {
            new Exercise("Z Exercise", "Description Z"),
            new Exercise("A Exercise", "Description A"),
            new Exercise("M Exercise", "Description M")
        };

        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        exerciseRepoMock.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        var handler = new GetAllExercisesQueryHandler(exerciseRepoMock.Object);
        var query = new GetAllExercisesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var exercisesList = result.Value.ToList();
        Assert.Equal(3, exercisesList.Count);
        Assert.Equal("A Exercise", exercisesList[0].Name);
        Assert.Equal("M Exercise", exercisesList[1].Name);
        Assert.Equal("Z Exercise", exercisesList[2].Name);
    }

    [Fact]
    public async Task Handle_WithEmptyList_ShouldReturnEmptyList()
    {
        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        exerciseRepoMock.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Exercise>());

        var handler = new GetAllExercisesQueryHandler(exerciseRepoMock.Object);
        var query = new GetAllExercisesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_WithManyExercises_ShouldReturnAllOrdered()
    {
        var exercises = Enumerable.Range(1, 100)
            .Select(i => new Exercise($"Exercise {i:D3}", $"Description {i}"))
            .ToList();

        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        exerciseRepoMock.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        var handler = new GetAllExercisesQueryHandler(exerciseRepoMock.Object);
        var query = new GetAllExercisesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var exercisesList = result.Value.ToList();
        Assert.Equal(100, exercisesList.Count);
        Assert.Equal("Exercise 001", exercisesList[0].Name);
    }

    [Fact]
    public async Task Handle_WithExercisesWithSameName_ShouldReturnAll()
    {
        var exercises = new List<Exercise>
        {
            new Exercise("Exercise", "Description 1"),
            new Exercise("Exercise", "Description 2"),
            new Exercise("Exercise", "Description 3")
        };

        var exerciseRepoMock = MockHelpers.CreateReadRepositoryMock<Exercise>();
        exerciseRepoMock.Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        var handler = new GetAllExercisesQueryHandler(exerciseRepoMock.Object);
        var query = new GetAllExercisesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count());
    }
}
