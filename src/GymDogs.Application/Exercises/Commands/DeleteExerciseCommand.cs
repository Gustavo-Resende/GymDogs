using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.Exercises;
using GymDogs.Domain.Exercises.Specification;

namespace GymDogs.Application.Exercises.Commands;

public record DeleteExerciseCommand(Guid ExerciseId) : ICommand<Result>;

internal class DeleteExerciseCommandHandler : ICommandHandler<DeleteExerciseCommand, Result>
{
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExerciseCommandHandler(
        IRepository<Exercise> exerciseRepository,
        IUnitOfWork unitOfWork)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteExerciseCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExerciseId == Guid.Empty)
        {
            return Result.NotFound("Exercise ID is required.");
        }

        var exercise = await _exerciseRepository.FirstOrDefaultAsync(
            new GetExerciseByIdSpec(request.ExerciseId),
            cancellationToken);

        if (exercise == null)
        {
            return Result.NotFound($"Exercise with ID {request.ExerciseId} not found.");
        }

        await _exerciseRepository.DeleteAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
