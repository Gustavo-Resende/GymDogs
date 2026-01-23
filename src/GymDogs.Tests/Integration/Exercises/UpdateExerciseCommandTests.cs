using Ardalis.Result;
using GymDogs.Application.Exercises.Commands;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.Exercises;
using GymDogs.Tests.Integration.Helpers;
using Moq;
using Xunit;

namespace GymDogs.Tests.Integration.Exercises;

public class UpdateExerciseCommandTests
{
    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateExercise()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        exerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(exercise.Id, "New Name", "New Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
        Assert.Equal("New Description", result.Value.Description);
        exerciseRepoMock.Verify(x => x.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentExercise_ShouldReturnNotFound()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync<Exercise>(null);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(Guid.NewGuid(), "New Name", "New Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithEmptyGuid_ShouldReturnNotFound()
    {
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(Guid.Empty, "New Name", "New Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_WithNullNameAndDescription_ShouldReturnInvalid()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(exercise.Id, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task Handle_WithOnlyName_ShouldUpdateName()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        exerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(exercise.Id, "New Name", null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithOnlyDescription_ShouldUpdateDescription()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        exerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(exercise.Id, null, "New Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Description", result.Value.Description);
    }

    [Fact]
    public async Task Handle_WithNameWithWhitespace_ShouldNormalizeName()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        exerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(exercise.Id, "  New Name  ", "Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WithMaxLengthName_ShouldUpdate()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var maxName = new string('a', 200);
        var exerciseRepoMock = MockHelpers.CreateRepositoryMock<Exercise>();
        var unitOfWorkMock = MockHelpers.CreateUnitOfWorkMock();

        exerciseRepoMock.SetupFirstOrDefaultAsync(exercise);
        exerciseRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new UpdateExerciseCommandHandler(exerciseRepoMock.Object, unitOfWorkMock.Object);
        var command = new UpdateExerciseCommand(exercise.Id, maxName, "Description");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(maxName, result.Value.Name);
    }
}
