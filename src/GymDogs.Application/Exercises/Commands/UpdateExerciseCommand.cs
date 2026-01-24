using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Commands;

public record UpdateExerciseCommand(Guid ExerciseId, string? Name, string? Description)
    : ICommand<Result<GetExerciseDto>>;

internal class UpdateExerciseCommandHandler : ICommandHandler<UpdateExerciseCommand, Result<GetExerciseDto>>
{
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public UpdateExerciseCommandHandler(
        IRepository<Exercise> exerciseRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<GetExerciseDto>> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExerciseId == Guid.Empty)
        {
            return Result<GetExerciseDto>.NotFound("Exercise ID is required.");
        }

        var exercise = await _exerciseRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetExerciseByIdSpec(request.ExerciseId),
            cancellationToken);

        if (exercise == null)
        {
            return Result<GetExerciseDto>.NotFound($"Exercise with ID {request.ExerciseId} not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) && request.Description == null)
        {
            return Result<GetExerciseDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Request", ErrorMessage = "At least one field must be provided for update." }
                });
        }

        var nameToUpdate = string.IsNullOrWhiteSpace(request.Name) ? exercise.Name : request.Name?.Trim() ?? string.Empty;
        var descriptionToUpdate = request.Description?.Trim();

        exercise.Update(nameToUpdate, descriptionToUpdate);
        await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(exercise.ToGetExerciseDto());
    }
}
