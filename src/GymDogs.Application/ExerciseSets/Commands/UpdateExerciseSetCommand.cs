using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Extensions;
using GymDogs.Domain.ExerciseSets;
using GymDogs.Domain.ExerciseSets.Specification;

namespace GymDogs.Application.ExerciseSets.Commands;

public record UpdateExerciseSetCommand(Guid ExerciseSetId, int? Reps, decimal? Weight)
    : ICommand<Result<GetExerciseSetDto>>;

internal class UpdateExerciseSetCommandHandler : ICommandHandler<UpdateExerciseSetCommand, Result<GetExerciseSetDto>>
{
    private readonly IRepository<ExerciseSet> _exerciseSetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExerciseSetCommandHandler(
        IRepository<ExerciseSet> exerciseSetRepository,
        IUnitOfWork unitOfWork)
    {
        _exerciseSetRepository = exerciseSetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetExerciseSetDto>> Handle(
        UpdateExerciseSetCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExerciseSetId == Guid.Empty)
        {
            return Result<GetExerciseSetDto>.NotFound("ExerciseSet ID is required.");
        }

        var exerciseSet = await _exerciseSetRepository.FirstOrDefaultAsync(
            new GetExerciseSetByIdSpec(request.ExerciseSetId),
            cancellationToken);

        if (exerciseSet == null)
        {
            return Result<GetExerciseSetDto>.NotFound($"ExerciseSet with ID {request.ExerciseSetId} not found.");
        }

        if (!request.Reps.HasValue && !request.Weight.HasValue)
        {
            return Result<GetExerciseSetDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Request", ErrorMessage = "At least one field (Reps or Weight) must be provided for update." }
                });
        }

        var repsToUpdate = request.Reps ?? exerciseSet.Reps;
        var weightToUpdate = request.Weight ?? exerciseSet.Weight;

        exerciseSet.Update(repsToUpdate, weightToUpdate);
        await _exerciseSetRepository.UpdateAsync(exerciseSet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(exerciseSet.ToGetExerciseSetDto());
    }
}
