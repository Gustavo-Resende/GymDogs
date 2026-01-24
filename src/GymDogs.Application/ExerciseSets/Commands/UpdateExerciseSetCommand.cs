using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Extensions;
using GymDogs.Domain.ExerciseSets;

namespace GymDogs.Application.ExerciseSets.Commands;

public record UpdateExerciseSetCommand(Guid ExerciseSetId, int? Reps, decimal? Weight, Guid? CurrentUserId = null)
    : ICommand<Result<GetExerciseSetDto>>;

internal class UpdateExerciseSetCommandHandler : ICommandHandler<UpdateExerciseSetCommand, Result<GetExerciseSetDto>>
{
    private readonly IRepository<ExerciseSet> _exerciseSetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public UpdateExerciseSetCommandHandler(
        IRepository<ExerciseSet> exerciseSetRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _exerciseSetRepository = exerciseSetRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetExerciseSetByIdSpec(request.ExerciseSetId),
            cancellationToken);

        if (exerciseSet == null)
        {
            return Result<GetExerciseSetDto>.NotFound($"ExerciseSet with ID {request.ExerciseSetId} not found.");
        }

        // Property-based authorization: Users can only update sets in their own workout folders
        if (!request.CurrentUserId.HasValue || exerciseSet.FolderExercise.WorkoutFolder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result<GetExerciseSetDto>.Forbidden("You can only update sets in your own workout folders.");
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
