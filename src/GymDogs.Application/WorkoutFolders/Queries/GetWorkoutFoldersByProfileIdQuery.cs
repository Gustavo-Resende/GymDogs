using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.WorkoutFolders.Queries;

public record GetWorkoutFoldersByProfileIdQuery(Guid ProfileId) : IQuery<Result<IEnumerable<GetWorkoutFolderDto>>>;

internal class GetWorkoutFoldersByProfileIdQueryHandler : IQueryHandler<GetWorkoutFoldersByProfileIdQuery, Result<IEnumerable<GetWorkoutFolderDto>>>
{
    private readonly IReadRepository<WorkoutFolder> _workoutFolderRepository;

    public GetWorkoutFoldersByProfileIdQueryHandler(IReadRepository<WorkoutFolder> workoutFolderRepository)
    {
        _workoutFolderRepository = workoutFolderRepository;
    }

    public async Task<Result<IEnumerable<GetWorkoutFolderDto>>> Handle(
        GetWorkoutFoldersByProfileIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.ProfileId == Guid.Empty)
        {
            return Result<IEnumerable<GetWorkoutFolderDto>>.NotFound("Profile ID is required.");
        }

        var folders = await _workoutFolderRepository.ListAsync(
            new GetWorkoutFoldersByProfileIdSpec(request.ProfileId),
            cancellationToken);

        var folderDtos = folders.Select(f => f.ToGetWorkoutFolderDto());

        return Result.Success(folderDtos);
    }
}
