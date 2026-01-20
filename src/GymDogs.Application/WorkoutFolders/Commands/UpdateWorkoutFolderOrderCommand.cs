using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.WorkoutFolders.Commands;

public record UpdateWorkoutFolderOrderCommand(Guid WorkoutFolderId, int Order)
    : ICommand<Result<GetWorkoutFolderDto>>;

internal class UpdateWorkoutFolderOrderCommandHandler : ICommandHandler<UpdateWorkoutFolderOrderCommand, Result<GetWorkoutFolderDto>>
{
    private readonly IRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkoutFolderOrderCommandHandler(
        IRepository<WorkoutFolder> workoutFolderRepository,
        IUnitOfWork unitOfWork)
    {
        _workoutFolderRepository = workoutFolderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetWorkoutFolderDto>> Handle(
        UpdateWorkoutFolderOrderCommand request,
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

        folder.UpdateOrder(request.Order);
        await _workoutFolderRepository.UpdateAsync(folder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(folder.ToGetWorkoutFolderDto());
    }
}
