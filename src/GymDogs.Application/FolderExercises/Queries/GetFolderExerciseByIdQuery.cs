using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Application.FolderExercises.Extensions;
using GymDogs.Domain.FolderExercises;

namespace GymDogs.Application.FolderExercises.Queries;

public record GetFolderExerciseByIdQuery(Guid FolderExerciseId) : IQuery<Result<GetFolderExerciseDto>>;

internal class GetFolderExerciseByIdQueryHandler : IQueryHandler<GetFolderExerciseByIdQuery, Result<GetFolderExerciseDto>>
{
    private readonly IReadRepository<FolderExercise> _folderExerciseRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetFolderExerciseByIdQueryHandler(
        IReadRepository<FolderExercise> folderExerciseRepository,
        ISpecificationFactory specificationFactory)
    {
        _folderExerciseRepository = folderExerciseRepository;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result<GetFolderExerciseDto>> Handle(
        GetFolderExerciseByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FolderExerciseId == Guid.Empty)
        {
            return Result<GetFolderExerciseDto>.NotFound("FolderExercise ID is required.");
        }

        var folderExercise = await _folderExerciseRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetFolderExerciseByIdSpec(request.FolderExerciseId),
            cancellationToken);

        if (folderExercise == null)
        {
            return Result<GetFolderExerciseDto>.NotFound($"FolderExercise with ID {request.FolderExerciseId} not found.");
        }

        return Result.Success(folderExercise.ToGetFolderExerciseDto());
    }
}
