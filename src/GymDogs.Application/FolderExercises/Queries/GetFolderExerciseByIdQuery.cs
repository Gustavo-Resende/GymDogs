using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Application.FolderExercises.Extensions;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.FolderExercises.Specification;

namespace GymDogs.Application.FolderExercises.Queries;

public record GetFolderExerciseByIdQuery(Guid FolderExerciseId) : IQuery<Result<GetFolderExerciseDto>>;

internal class GetFolderExerciseByIdQueryHandler : IQueryHandler<GetFolderExerciseByIdQuery, Result<GetFolderExerciseDto>>
{
    private readonly IReadRepository<FolderExercise> _folderExerciseRepository;

    public GetFolderExerciseByIdQueryHandler(IReadRepository<FolderExercise> folderExerciseRepository)
    {
        _folderExerciseRepository = folderExerciseRepository;
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
            new GetFolderExerciseByIdSpec(request.FolderExerciseId),
            cancellationToken);

        if (folderExercise == null)
        {
            return Result<GetFolderExerciseDto>.NotFound($"FolderExercise with ID {request.FolderExerciseId} not found.");
        }

        return Result.Success(folderExercise.ToGetFolderExerciseDto());
    }
}
