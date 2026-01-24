using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.WorkoutFolders;

namespace GymDogs.Application.WorkoutFolders.Commands;

public record DeleteWorkoutFolderCommand(Guid WorkoutFolderId, Guid? CurrentUserId = null) : ICommand<Result>;

internal class DeleteWorkoutFolderCommandHandler : ICommandHandler<DeleteWorkoutFolderCommand, Result>
{
    private readonly IRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public DeleteWorkoutFolderCommandHandler(
        IRepository<WorkoutFolder> workoutFolderRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _workoutFolderRepository = workoutFolderRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result> Handle(
        DeleteWorkoutFolderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.WorkoutFolderId == Guid.Empty)
        {
            return Result.NotFound("WorkoutFolder ID is required.");
        }

        var folder = await _workoutFolderRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetWorkoutFolderByIdSpec(request.WorkoutFolderId),
            cancellationToken);

        if (folder == null)
        {
            return Result.NotFound($"WorkoutFolder with ID {request.WorkoutFolderId} not found.");
        }

        // Property-based authorization: Users can only delete their own workout folders
        if (!request.CurrentUserId.HasValue || folder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result.Forbidden("You can only delete your own workout folders.");
        }

        await _workoutFolderRepository.DeleteAsync(folder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
