using Ardalis.Result;
using GymDogs.Application.Exercises.Commands;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.Exercises;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Exercises;

public class CreateExerciseCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldCreateExercise()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand("Bench Press", "Chest exercise");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Bench Press", result.Value.Name);
        Assert.Equal("Chest exercise", result.Value.Description);
        exerciseRepoMock.Verify(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ShouldCreateExercise()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand("Squat", null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithNameWithWhitespace_ShouldNormalizeName()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand("  Bench Press  ", "Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Bench Press", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithDescriptionWithWhitespace_ShouldNormalizeDescription()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand("Squat", "  Description  ");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Description", result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithMaxLengthName_ShouldCreateExercise()
    {
        var maxName = new string('a', 200);
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand(maxName, "Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(maxName, result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithMaxLengthDescription_ShouldCreateExercise()
    {
        var maxDescription = new string('a', 1000);
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand("Test", maxDescription);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(maxDescription, result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithUnicodeCharacters_ShouldCreateExercise()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.Setup(x => x.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise e, CancellationToken ct) => e);

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand("Exercício de Peito 🏋️", "Descrição com emojis: 💪🔥");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Exercício de Peito 🏋️", result.Value.Name);
        Assert.Equal("Descrição com emojis: 💪🔥", result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithNullName_ShouldThrowException()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new CreateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new CreateExerciseCommand(null!, "Description");

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }
}
