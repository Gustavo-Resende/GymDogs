using GymDogs.Domain.ExerciseSets;
using Xunit;

namespace GymDogs.Tests.ExerciseSets;

public class ExerciseSetTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateExerciseSet()
    {
        var folderExerciseId = Guid.NewGuid();
        var setNumber = 1;
        var reps = 10;
        var weight = 50.5m;

        var exerciseSet = new ExerciseSet(folderExerciseId, setNumber, reps, weight);

        Assert.Equal(folderExerciseId, exerciseSet.FolderExerciseId);
        Assert.Equal(setNumber, exerciseSet.SetNumber);
        Assert.Equal(reps, exerciseSet.Reps);
        Assert.Equal(weight, exerciseSet.Weight);
        Assert.NotEqual(Guid.Empty, exerciseSet.Id);
        Assert.True(exerciseSet.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_WithDefaultFolderExerciseId_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new ExerciseSet(Guid.Empty, 1, 10, 50));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidSetNumber_ShouldThrowException(int setNumber)
    {
        Assert.Throws<ArgumentException>(() => new ExerciseSet(Guid.NewGuid(), setNumber, 10, 50));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidReps_ShouldThrowException(int reps)
    {
        Assert.Throws<ArgumentException>(() => new ExerciseSet(Guid.NewGuid(), 1, reps, 50));
    }

    [Fact]
    public void Constructor_WithRepsTooHigh_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExerciseSet(Guid.NewGuid(), 1, 1001, 50));
    }

    [Fact]
    public void Constructor_WithRepsAtMax_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 1000, 50);

        Assert.Equal(1000, exerciseSet.Reps);
    }

    [Fact]
    public void Constructor_WithRepsAtMin_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 1, 50);

        Assert.Equal(1, exerciseSet.Reps);
    }

    [Fact]
    public void Constructor_WithNegativeWeight_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new ExerciseSet(Guid.NewGuid(), 1, 10, -1));
    }

    [Fact]
    public void Constructor_WithZeroWeight_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 0);

        Assert.Equal(0, exerciseSet.Weight);
    }

    [Fact]
    public void Constructor_WithWeightTooHigh_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExerciseSet(Guid.NewGuid(), 1, 10, 10001));
    }

    [Fact]
    public void Constructor_WithWeightAtMax_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 10000);

        Assert.Equal(10000, exerciseSet.Weight);
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        var newReps = 12;
        var newWeight = 55.5m;
        var oldUpdatedAt = exerciseSet.LastUpdatedAt;

        Thread.Sleep(10);
        exerciseSet.Update(newReps, newWeight);

        Assert.Equal(newReps, exerciseSet.Reps);
        Assert.Equal(newWeight, exerciseSet.Weight);
        Assert.True(exerciseSet.LastUpdatedAt > oldUpdatedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Update_WithInvalidReps_ShouldThrowException(int reps)
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        Assert.Throws<ArgumentException>(() => exerciseSet.Update(reps, 50));
    }

    [Fact]
    public void Update_WithRepsTooHigh_ShouldThrowException()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        Assert.Throws<ArgumentOutOfRangeException>(() => exerciseSet.Update(1001, 50));
    }

    [Fact]
    public void Update_WithNegativeWeight_ShouldThrowException()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        Assert.Throws<ArgumentException>(() => exerciseSet.Update(10, -1));
    }

    [Fact]
    public void Update_WithWeightTooHigh_ShouldThrowException()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        Assert.Throws<ArgumentOutOfRangeException>(() => exerciseSet.Update(10, 10001));
    }

    [Fact]
    public void Update_WithZeroWeight_ShouldUpdateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        exerciseSet.Update(10, 0);

        Assert.Equal(0, exerciseSet.Weight);
    }

    [Fact]
    public void Constructor_WithSetNumberAtMaxInt_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), int.MaxValue, 10, 50);

        Assert.Equal(int.MaxValue, exerciseSet.SetNumber);
    }

    [Fact]
    public void Constructor_WithRepsAtMaxInt_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExerciseSet(Guid.NewGuid(), 1, int.MaxValue, 50));
    }

    [Fact]
    public void Update_WithRepsAtMaxInt_ShouldThrowException()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        Assert.Throws<ArgumentOutOfRangeException>(() => exerciseSet.Update(int.MaxValue, 50));
    }

    [Fact]
    public void Constructor_WithWeightAtDecimalMax_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExerciseSet(Guid.NewGuid(), 1, 10, decimal.MaxValue));
    }

    [Fact]
    public void Update_WithWeightAtDecimalMax_ShouldThrowException()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        Assert.Throws<ArgumentOutOfRangeException>(() => exerciseSet.Update(10, decimal.MaxValue));
    }

    [Fact]
    public void Constructor_WithVerySmallWeight_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 0.0001m);

        Assert.Equal(0.0001m, exerciseSet.Weight);
    }

    [Fact]
    public void Update_WithVerySmallWeight_ShouldUpdate()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        exerciseSet.Update(10, 0.0001m);

        Assert.Equal(0.0001m, exerciseSet.Weight);
    }

    [Fact]
    public void Constructor_WithHighPrecisionWeight_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 12.3456789m);

        Assert.Equal(12.3456789m, exerciseSet.Weight);
    }

    [Fact]
    public void Update_WithHighPrecisionWeight_ShouldUpdate()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        exerciseSet.Update(10, 99.9999999m);

        Assert.Equal(99.9999999m, exerciseSet.Weight);
    }

    [Fact]
    public void MultipleUpdates_ShouldUpdateTimestampEachTime()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        var firstUpdate = exerciseSet.LastUpdatedAt;

        Thread.Sleep(10);
        exerciseSet.Update(12, 55);
        var secondUpdate = exerciseSet.LastUpdatedAt;

        Thread.Sleep(10);
        exerciseSet.Update(15, 60);
        var thirdUpdate = exerciseSet.LastUpdatedAt;

        Assert.True(secondUpdate > firstUpdate);
        Assert.True(thirdUpdate > secondUpdate);
    }

    [Fact]
    public void Update_WithSameValues_ShouldUpdateTimestamp()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 50);
        var oldUpdatedAt = exerciseSet.LastUpdatedAt;

        Thread.Sleep(10);
        exerciseSet.Update(10, 50);

        Assert.True(exerciseSet.LastUpdatedAt > oldUpdatedAt);
    }

    [Fact]
    public void Constructor_WithRepsAtBoundary999_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 999, 50);

        Assert.Equal(999, exerciseSet.Reps);
    }

    [Fact]
    public void Constructor_WithWeightAtBoundary9999_ShouldCreateExerciseSet()
    {
        var exerciseSet = new ExerciseSet(Guid.NewGuid(), 1, 10, 9999.99m);

        Assert.Equal(9999.99m, exerciseSet.Weight);
    }
}
