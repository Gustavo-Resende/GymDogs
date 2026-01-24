using GymDogs.Domain.Exercises;
using Xunit;

namespace GymDogs.Tests.Exercises;

public class ExerciseTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldCreateExercise()
    {
        var name = "Bench Press";
        var exercise = new Exercise(name);

        Assert.Equal(name, exercise.Name);
        Assert.Null(exercise.Description);
        Assert.NotEqual(Guid.Empty, exercise.Id);
        Assert.True(exercise.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithNameAndDescription_ShouldCreateExercise()
    {
        var name = "Squat";
        var description = "Lower body exercise";
        var exercise = new Exercise(name, description);

        Assert.Equal(name, exercise.Name);
        Assert.Equal(description, exercise.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhiteSpaceName_ShouldThrowException(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Exercise(name!));
    }

    [Fact]
    public void Constructor_WithNameTooLong_ShouldThrowException()
    {
        var longName = new string('a', 201);
        Assert.Throws<ArgumentException>(() => new Exercise(longName));
    }

    [Fact]
    public void Constructor_WithDescriptionTooLong_ShouldThrowException()
    {
        var longDescription = new string('a', 1001);
        Assert.Throws<ArgumentException>(() => new Exercise("Test", longDescription));
    }

    [Fact]
    public void Constructor_WithMaxLengthName_ShouldCreateExercise()
    {
        var maxName = new string('a', 200);
        var exercise = new Exercise(maxName);

        Assert.Equal(maxName, exercise.Name);
    }

    [Fact]
    public void Constructor_WithMaxLengthDescription_ShouldCreateExercise()
    {
        var maxDescription = new string('a', 1000);
        var exercise = new Exercise("Test", maxDescription);

        Assert.Equal(maxDescription, exercise.Description);
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldSetNullDescription()
    {
        var exercise = new Exercise("Test", null);

        Assert.Null(exercise.Description);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ShouldSetEmptyString()
    {
        var exercise = new Exercise("Test", "");

        Assert.Equal("", exercise.Description);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateExercise()
    {
        var exercise = new Exercise("Old Name", "Old Description");
        var newName = "New Name";
        var newDescription = "New Description";
        var oldUpdatedAt = exercise.LastUpdatedAt;

        Thread.Sleep(10);
        exercise.Update(newName, newDescription);

        Assert.Equal(newName, exercise.Name);
        Assert.Equal(newDescription, exercise.Description);
        Assert.True(exercise.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void Update_WithNullDescription_ShouldSetNullDescription()
    {
        var exercise = new Exercise("Test", "Description");
        exercise.Update("Test", null);

        Assert.Null(exercise.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithNullOrWhiteSpaceName_ShouldThrowException(string? name)
    {
        var exercise = new Exercise("Test");
        Assert.ThrowsAny<ArgumentException>(() => exercise.Update(name!, "Description"));
    }

    [Fact]
    public void Update_WithNameTooLong_ShouldThrowException()
    {
        var exercise = new Exercise("Test");
        var longName = new string('a', 201);
        Assert.Throws<ArgumentException>(() => exercise.Update(longName, "Description"));
    }

    [Fact]
    public void Update_WithDescriptionTooLong_ShouldThrowException()
    {
        var exercise = new Exercise("Test");
        var longDescription = new string('a', 1001);
        Assert.Throws<ArgumentException>(() => exercise.Update("Test", longDescription));
    }

    [Fact]
    public void Constructor_WithSingleCharacterName_ShouldCreateExercise()
    {
        var exercise = new Exercise("A");

        Assert.Equal("A", exercise.Name);
    }

    [Fact]
    public void Update_WithSingleCharacterName_ShouldUpdate()
    {
        var exercise = new Exercise("Test");
        exercise.Update("X", null);

        Assert.Equal("X", exercise.Name);
    }

    [Fact]
    public void Constructor_WithUnicodeCharacters_ShouldCreateExercise()
    {
        var name = "Exercício de Peito 🏋️";
        var description = "Descrição com emojis: 💪🔥";
        var exercise = new Exercise(name, description);

        Assert.Equal(name, exercise.Name);
        Assert.Equal(description, exercise.Description);
    }

    [Fact]
    public void Update_WithUnicodeCharacters_ShouldUpdate()
    {
        var exercise = new Exercise("Test");
        var name = "Exercício de Pernas 🦵";
        var description = "Descrição: 💪";
        exercise.Update(name, description);

        Assert.Equal(name, exercise.Name);
        Assert.Equal(description, exercise.Description);
    }

    [Fact]
    public void Constructor_WithWhitespaceOnlyDescription_ShouldSetWhitespace()
    {
        var exercise = new Exercise("Test", "   ");

        Assert.Equal("   ", exercise.Description);
    }

    [Fact]
    public void Update_WithWhitespaceOnlyDescription_ShouldSetWhitespace()
    {
        var exercise = new Exercise("Test", "Description");
        exercise.Update("Test", "   ");

        Assert.Equal("   ", exercise.Description);
    }

    [Fact]
    public void Update_WithSameValues_ShouldUpdateTimestamp()
    {
        var exercise = new Exercise("Test", "Description");
        var oldUpdatedAt = exercise.LastUpdatedAt;

        Thread.Sleep(10);
        exercise.Update("Test", "Description");

        Assert.True(exercise.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void MultipleUpdates_ShouldUpdateTimestampEachTime()
    {
        var exercise = new Exercise("Name1", "Desc1");
        var firstUpdate = exercise.LastUpdatedAt;

        Thread.Sleep(10);
        exercise.Update("Name2", "Desc2");
        var secondUpdate = exercise.LastUpdatedAt;

        Thread.Sleep(10);
        exercise.Update("Name3", "Desc3");
        var thirdUpdate = exercise.LastUpdatedAt;

        Assert.True(secondUpdate > firstUpdate);
        Assert.True(thirdUpdate > secondUpdate);
    }

    [Fact]
    public void Constructor_WithSpecialCharacters_ShouldCreateExercise()
    {
        var name = "Exercise (Upper Body) - Day 1";
        var description = "Description with: special chars, numbers 123, and symbols!";
        var exercise = new Exercise(name, description);

        Assert.Equal(name, exercise.Name);
        Assert.Equal(description, exercise.Description);
    }
}
