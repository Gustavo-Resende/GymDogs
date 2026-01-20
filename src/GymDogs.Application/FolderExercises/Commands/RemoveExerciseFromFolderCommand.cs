using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Domain.FolderExercises;
using GymDogs.Domain.FolderExercises.Specification;

namespace GymDogs.Application.FolderExercises.Commands;

public record RemoveExerciseFromFolderCommand(Guid FolderExerciseId) : ICommand<Result>;

internal class RemoveExerciseFromFolderCommandHandler : ICommandHandler<RemoveExerciseFromFolderCommand, Result>
{
    private readonly IRepository<FolderExercise> _folderExerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveExerciseFromFolderCommandHandler(
        IRepository<FolderExercise> folderExerciseRepository,
        IUnitOfWork unitOfWork)
    {
        _folderExerciseRepository = folderExerciseRepository;
        _unitOfWork = unitOfWork;
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
            new GetFolderExerciseByIdSpec(request.FolderExerciseId),
            cancellationToken);

        if (folderExercise == null)
        {
            return Result.NotFound($"FolderExercise with ID {request.FolderExerciseId} not found.");
        }

        await _folderExerciseRepository.DeleteAsync(folderExercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
