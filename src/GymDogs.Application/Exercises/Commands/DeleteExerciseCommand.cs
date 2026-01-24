using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Commands;

public record DeleteExerciseCommand(Guid ExerciseId) : ICommand<Result>;

internal class DeleteExerciseCommandHandler : ICommandHandler<DeleteExerciseCommand, Result>
{
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public DeleteExerciseCommandHandler(
        IRepository<Exercise> exerciseRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetExerciseByIdSpec(request.ExerciseId),
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
