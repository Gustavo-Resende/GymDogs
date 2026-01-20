using Ardalis.Result;
using GymDogs.Application.Common;
using GymDogs.Application.Interfaces;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Extensions;
using GymDogs.Domain.Exercises;

namespace GymDogs.Application.Exercises.Commands;

public record CreateExerciseCommand(string Name, string? Description)
    : ICommand<Result<CreateExerciseDto>>;

internal class CreateExerciseCommandHandler : ICommandHandler<CreateExerciseCommand, Result<CreateExerciseDto>>
{
    private readonly IRepository<Exercise> _exerciseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExerciseCommandHandler(
        IRepository<Exercise> exerciseRepository,
        IUnitOfWork unitOfWork)
    {
        _exerciseRepository = exerciseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateExerciseDto>> Handle(
        CreateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var exercise = new Exercise(
            request.Name?.Trim() ?? string.Empty,
            request.Description?.Trim());

        await _exerciseRepository.AddAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(exercise.ToCreateExerciseDto());
    }
}
