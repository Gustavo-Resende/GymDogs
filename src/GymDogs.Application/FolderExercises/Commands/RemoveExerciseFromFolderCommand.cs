using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Common.Specification;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.FolderExercises;

namespace GymDogs.Application.FolderExercises.Commands;

public record RemoveExerciseFromFolderCommand(Guid FolderExerciseId, Guid? CurrentUserId = null) : ICommand<Result>;

internal class RemoveExerciseFromFolderCommandHandler : ICommandHandler<RemoveExerciseFromFolderCommand, Result>
{
    private readonly IRepository<FolderExercise> _folderExerciseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISpecificationFactory _specificationFactory;

    public RemoveExerciseFromFolderCommandHandler(
        IRepository<FolderExercise> folderExerciseRepository,
        IUnitOfWork unitOfWork,
        ISpecificationFactory specificationFactory)
    {
        _folderExerciseRepository = folderExerciseRepository;
        _unitOfWork = unitOfWork;
        _specificationFactory = specificationFactory;
    }

    public async Task<Result> Handle(
        RemoveExerciseFromFolderCommand request,
        CancellationToken cancellationToken)
    {
        if (request.FolderExerciseId == Guid.Empty)
        {
            return Result.NotFound("FolderExercise ID is required.");
        }

        var folderExercise = await _folderExerciseRepository.FirstOrDefaultAsync(
            _specificationFactory.CreateGetFolderExerciseByIdSpec(request.FolderExerciseId),
            cancellationToken);

        if (folderExercise == null)
        {
            return Result.NotFound($"FolderExercise with ID {request.FolderExerciseId} not found.");
        }

        // Property-based authorization: Users can only remove exercises from their own workout folders
        if (!request.CurrentUserId.HasValue || folderExercise.WorkoutFolder.Profile.UserId != request.CurrentUserId.Value)
        {
            return Result.Forbidden("You can only remove exercises from your own workout folders.");
        }

        await _folderExerciseRepository.DeleteAsync(folderExercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
