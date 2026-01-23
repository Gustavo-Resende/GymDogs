using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.ExerciseSets.Specification;

namespace GymDogs.Application.ExerciseSets.Commands;

public record DeleteExerciseSetCommand(Guid ExerciseSetId, Guid? CurrentUserId = null) : ICommand<Result>;

internal class DeleteExerciseSetCommandHandler : ICommandHandler<DeleteExerciseSetCommand, Result>
{
    private readonly IRepository<ExerciseSet> _exerciseSetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExerciseSetCommandHandler(
        IRepository<ExerciseSet> exerciseSetRepository,
        IUnitOfWork unitOfWork)
    {
        _exerciseSetRepository = exerciseSetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteExerciseSetCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExerciseSetId == Guid.Empty)
        {
            return Result.NotFound("ExerciseSet ID is required.");
        }

        var exerciseSet = await _exerciseSetRepository.FirstOrDefaultAsync(
            new GetExerciseSetByIdSpec(request.ExerciseSetId),
            cancellationToken);

        if (exerciseSet == null)
        {
            return Result.NotFound($"ExerciseSet with ID {request.ExerciseSetId} not found.");
        }

        // Property-based authorization: Users can only delete sets in their own workout folders
        if (!request.CurrentUserId.HasValue || exerciseSet.FolderExercise.WorkoutFolder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result.Forbidden("You can only delete sets in your own workout folders.");
        }

        await _exerciseSetRepository.DeleteAsync(exerciseSet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
