using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Application.FolderExercises.Extensions;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.FolderExercises.Specification;

namespace GymDogs.Application.FolderExercises.Commands;

public record UpdateFolderExerciseOrderCommand(Guid FolderExerciseId, int Order, Guid? CurrentUserId = null)
    : ICommand<Result<GetFolderExerciseDto>>;

internal class UpdateFolderExerciseOrderCommandHandler : ICommandHandler<UpdateFolderExerciseOrderCommand, Result<GetFolderExerciseDto>>
{
    private readonly IRepository<FolderExercise> _folderExerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFolderExerciseOrderCommandHandler(
        IRepository<FolderExercise> folderExerciseRepository,
        IUnitOfWork unitOfWork)
    {
        _folderExerciseRepository = folderExerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GetFolderExerciseDto>> Handle(
        UpdateFolderExerciseOrderCommand request,
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

        // Property-based authorization: Users can only update order of exercises in their own workout folders
        if (!request.CurrentUserId.HasValue || folderExercise.WorkoutFolder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result<GetFolderExerciseDto>.Forbidden("You can only update order of exercises in your own workout folders.");
        }

        folderExercise.UpdateOrder(request.Order);
        await _folderExerciseRepository.UpdateAsync(folderExercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(folderExercise.ToGetFolderExerciseDto());
    }
}
