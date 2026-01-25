using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Queries;

/// <summary>
/// Query para buscar exercícios disponíveis (não adicionados) em uma pasta de treino.
/// </summary>
public record GetAvailableExercisesForFolderQuery(Guid WorkoutFolderId) : IQuery<Result<IEnumerable<GetExerciseDto>>>;

internal class GetAvailableExercisesForFolderQueryHandler : IQueryHandler<GetAvailableExercisesForFolderQuery, Result<IEnumerable<GetExerciseDto>>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetAvailableExercisesForFolderQueryHandler(
        IReadRepository<Exercise> exerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<IEnumerable<GetExerciseDto>>> Handle(
        GetAvailableExercisesForFolderQuery request,
        CancellationToken cancellationToken)
    {
        if (request.WorkoutFolderId == Guid.Empty)
        {
            return Result<IEnumerable<GetExerciseDto>>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = nameof(request.WorkoutFolderId), ErrorMessage = "WorkoutFolder ID is required and cannot be empty." }
                });
        }

        var exercises = await _exerciseRepository.ListAsync(
            _specificationFactory.CreateGetAvailableExercisesForFolderSpec(request.WorkoutFolderId),
            cancellationToken);

        var exerciseDtos = exercises.Select(e => e.ToGetExerciseDto());

        return Result.Success(exerciseDtos);
    }
}
