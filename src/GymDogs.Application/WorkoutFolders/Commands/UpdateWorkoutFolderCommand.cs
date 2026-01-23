using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Extensions;
using GymDogs.Domain.WorkoutFolders;
using GymDogs.Domain.WorkoutFolders.Specification;

namespace GymDogs.Application.WorkoutFolders.Commands;

public record UpdateWorkoutFolderCommand(Guid WorkoutFolderId, string? Name, string? Description, Guid? CurrentUserId = null)
    : ICommand<Result<GetWorkoutFolderDto>>;

internal class UpdateWorkoutFolderCommandHandler : ICommandHandler<UpdateWorkoutFolderCommand, Result<GetWorkoutFolderDto>>
{
    private readonly IRepository<WorkoutFolder> _workoutFolderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkoutFolderCommandHandler(
        IRepository<WorkoutFolder> workoutFolderRepository,
        IUnitOfWork unitOfWork)
    {
        _workoutFolderRepository = workoutFolderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetWorkoutFolderDto>> Handle(
        UpdateWorkoutFolderCommand request,
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

        // Property-based authorization: Users can only update their own workout folders
        if (!request.CurrentUserId.HasValue || folder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result<GetWorkoutFolderDto>.Forbidden("You can only update your own workout folders.");
        }

        if (string.IsNullOrWhiteSpace(request.Name) && request.Description == null)
        {
            return Result<GetWorkoutFolderDto>.Invalid(
                new List<ValidationError>
                {
                    new() { Identifier = "Name", ErrorMessage = "At least one field must be provided for update." }
                });
        }

        var nameToUpdate = string.IsNullOrWhiteSpace(request.Name) ? folder.Name : request.Name?.Trim() ?? string.Empty;
        var descriptionToUpdate = request.Description?.Trim();

        folder.Update(nameToUpdate, descriptionToUpdate);
        await _workoutFolderRepository.UpdateAsync(folder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(folder.ToGetWorkoutFolderDto());
    }
}
