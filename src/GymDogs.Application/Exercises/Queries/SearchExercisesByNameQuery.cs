using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Queries;

public record SearchExercisesByNameQuery(string SearchTerm) : IQuery<Result<IEnumerable<GetExerciseDto>>>;

internal class SearchExercisesByNameQueryHandler : IQueryHandler<SearchExercisesByNameQuery, Result<IEnumerable<GetExerciseDto>>>
{
    private readonly IReadRepository<Exercise> _exerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public SearchExercisesByNameQueryHandler(
        IReadRepository<Exercise> exerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _exerciseRepository = exerciseRepository;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<IEnumerable<GetExerciseDto>>> Handle(
        SearchExercisesByNameQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return Result<IEnumerable<GetExerciseDto>>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "SearchTerm", ErrorMessage = "Search term is required." }
                });
        }

        var exercises = await _exerciseRepository.ListAsync(
            _specificationFactory.CreateSearchExercisesByNameSpec(request.SearchTerm),
            cancellationToken);

        var exerciseDtos = exercises.Select(e => e.ToGetExerciseDto());

        return Result.Success(exerciseDtos);
    }
}
