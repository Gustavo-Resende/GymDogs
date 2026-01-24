using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.WorkoutFolders;

namespace GymDogs.Application.WorkoutFolders.Queries;

public record GetWorkoutFolderByIdQuery(Guid WorkoutFolderId) : IQuery<Result<GetWorkoutFolderDto>>;

internal class GetWorkoutFolderByIdQueryHandler : IQueryHandler<GetWorkoutFolderByIdQuery, Result<GetWorkoutFolderDto>>
{
    private readonly IReadRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly ISpecificationFactory _specificationFactory;

    public GetWorkoutFolderByIdQueryHandler(
        IReadRepository<WorkoutFolder> workoutFolderRepository,
        ISpecificationFactory specificationFactory)
    {
        _workoutFolderRepository = workoutFolderRepository;
        _specificationFactory = specificationFactory;
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
            _specificationFactory.CreateGetWorkoutFolderByIdSpec(request.WorkoutFolderId),
            cancellationToken);

        if (folder == null)
        {
            return Result<GetWorkoutFolderDto>.NotFound($"WorkoutFolder with ID {request.WorkoutFolderId} not found.");
        }

        return Result.Success(folder.ToGetWorkoutFolderDto());
    }
}
