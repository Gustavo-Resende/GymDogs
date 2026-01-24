using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Application.FolderExercises.Extensions;
using GymDogs.Domain.FolderExercises;

namespace GymDogs.Application.FolderExercises.Queries;

public record GetExercisesByFolderIdQuery(Guid WorkoutFolderId) : IQuery<Result<IEnumerable<GetFolderExerciseDto>>>;

internal class GetExercisesByFolderIdQueryHandler : IQueryHandler<GetExercisesByFolderIdQuery, Result<IEnumerable<GetFolderExerciseDto>>>
{
    private readonly IReadRepository<FolderExercise> _folderExerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetExercisesByFolderIdQueryHandler(
        IReadRepository<FolderExercise> folderExerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _folderExerciseRepository = folderExerciseRepository;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<IEnumerable<GetFolderExerciseDto>>> Handle(
        GetExercisesByFolderIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.WorkoutFolderId == Guid.Empty)
        {
            return Result<IEnumerable<GetFolderExerciseDto>>.NotFound("WorkoutFolder ID is required.");
        }

        var folderExercises = await _folderExerciseRepository.ListAsync(
            _specificationFactory.CreateGetFolderExercisesByFolderIdSpec(request.WorkoutFolderId),
            cancellationToken);

        var folderExerciseDtos = folderExercises.Select(fe => fe.ToGetFolderExerciseDto());

        return Result.Success(folderExerciseDtos);
    }
}
