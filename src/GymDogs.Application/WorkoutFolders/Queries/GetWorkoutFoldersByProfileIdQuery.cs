using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.WorkoutFolders;

namespace GymDogs.Application.WorkoutFolders.Queries;

public record GetWorkoutFoldersByProfileIdQuery(Guid ProfileId) : IQuery<Result<IEnumerable<GetWorkoutFolderDto>>>;

internal class GetWorkoutFoldersByProfileIdQueryHandler : IQueryHandler<GetWorkoutFoldersByProfileIdQuery, Result<IEnumerable<GetWorkoutFolderDto>>>
{
    private readonly IReadRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetWorkoutFoldersByProfileIdQueryHandler(
        IReadRepository<WorkoutFolder> workoutFolderRepository,
        ISpecificationFactory specificationFactory)
    {
        _workoutFolderRepository = workoutFolderRepository;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetWorkoutFoldersByProfileIdSpec(request.ProfileId),
            cancellationToken);

        var folderDtos = folders.Select(f => f.ToGetWorkoutFolderDto());

        return Result.Success(folderDtos);
    }
}
