using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.WorkoutFolders.Queries;

public record GetWorkoutFolderByIdQuery(Guid WorkoutFolderId) : IQuery<Result<GetWorkoutFolderDto>>;

internal class GetWorkoutFolderByIdQueryHandler : IQueryHandler<GetWorkoutFolderByIdQuery, Result<GetWorkoutFolderDto>>
{
    private readonly IReadRepository<WorkoutFolder> _workoutFolderRepository;

    public GetWorkoutFolderByIdQueryHandler(IReadRepository<WorkoutFolder> workoutFolderRepository)
    {
        _workoutFolderRepository = workoutFolderRepository;
    }

    public async Task<Result<GetWorkoutFolderDto>> Handle(
        GetWorkoutFolderByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.WorkoutFolderId == Guid.Empty)
        {
            return Result<GetWorkoutFolderDto>.NotFound("WorkoutFolder ID is required.");
        }

        var folder = await _workoutFolderRepository.FirstOrDefaultAsync(
            new GetWorkoutFolderByIdSpec(request.WorkoutFolderId),
            cancellationToken);

        if (folder == null)
        {
            return Result<GetWorkoutFolderDto>.NotFound($"WorkoutFolder with ID {request.WorkoutFolderId} not found.");
        }

        return Result.Success(folder.ToGetWorkoutFolderDto());
    }
}
