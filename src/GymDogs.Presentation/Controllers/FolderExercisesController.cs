using GymDogs.Application.FolderExercises.Commands;
using GymDogs.Application.FolderExercises.Dtos;
using GymDogs.Application.FolderExercises.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento de exercícios dentro de pastas de treino
/// </summary>
[ApiController]
[Route("api/workout-folders/{workoutFolderId}/exercises")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class FolderExercisesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FolderExercisesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adiciona um exercício a uma pasta de treino
    /// </summary>
    /// <param name="workoutFolderId">ID da pasta de treino</param>
    /// <param name="request">Dados do exercício a ser adicionado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Exercício adicionado à pasta</returns>
    /// <response code="201">Exercício adicionado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Pasta de treino ou exercício não encontrado</response>
    /// <response code="409">Exercício já está na pasta</response>
    [HttpPost]
    [ProducesResponseType(typeof(AddExerciseToFolderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AddExerciseToFolderDto>> AddExerciseToFolder(
        [FromRoute] Guid workoutFolderId,
        [FromBody] AddExerciseToFolderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddExerciseToFolderCommand(
            workoutFolderId,
            request.ExerciseId,
            request.Order
        );
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista todos os exercícios de uma pasta de treino
    /// </summary>
    /// <param name="workoutFolderId">ID da pasta de treino</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de exercícios da pasta</returns>
    /// <response code="200">Lista de exercícios retornada com sucesso</response>
    /// <response code="404">Pasta de treino não encontrada</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetFolderExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetFolderExerciseDto>>> GetExercisesByFolderId(
        [FromRoute] Guid workoutFolderId,
        CancellationToken cancellationToken)
    {
        var query = new GetExercisesByFolderIdQuery(workoutFolderId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém um exercício de pasta pelo ID
    /// </summary>
    /// <param name="workoutFolderId">ID da pasta de treino</param>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do exercício na pasta</returns>
    /// <response code="200">Exercício encontrado</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpGet("{folderExerciseId}")]
    [ProducesResponseType(typeof(GetFolderExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetFolderExerciseDto>> GetFolderExerciseById(
        [FromRoute] Guid workoutFolderId,
        [FromRoute] Guid folderExerciseId,
        CancellationToken cancellationToken)
    {
        var query = new GetFolderExerciseByIdQuery(folderExerciseId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza a ordem de um exercício na pasta
    /// </summary>
    /// <param name="workoutFolderId">ID da pasta de treino</param>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="request">Nova ordem</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Exercício atualizado</returns>
    /// <response code="200">Ordem atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpPut("{folderExerciseId}/order")]
    [ProducesResponseType(typeof(GetFolderExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetFolderExerciseDto>> UpdateFolderExerciseOrder(
        [FromRoute] Guid workoutFolderId,
        [FromRoute] Guid folderExerciseId,
        [FromBody] UpdateFolderExerciseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateFolderExerciseOrderCommand(folderExerciseId, request.Order);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Remove um exercício de uma pasta de treino
    /// </summary>
    /// <param name="workoutFolderId">ID da pasta de treino</param>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Exercício removido com sucesso</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpDelete("{folderExerciseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveExerciseFromFolder(
        [FromRoute] Guid workoutFolderId,
        [FromRoute] Guid folderExerciseId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveExerciseFromFolderCommand(folderExerciseId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO para adicionar exercício a uma pasta
/// </summary>
public record AddExerciseToFolderRequest
{
    /// <summary>
    /// ID do exercício a ser adicionado
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid ExerciseId { get; init; }

    /// <summary>
    /// Ordem de exibição do exercício na pasta (padrão: 0)
    /// </summary>
    /// <example>1</example>
    public int Order { get; init; } = 0;
}

/// <summary>
/// Request DTO para atualização de ordem do exercício na pasta
/// </summary>
public record UpdateFolderExerciseOrderRequest
{
    /// <summary>
    /// Nova ordem de exibição
    /// </summary>
    /// <example>2</example>
    public int Order { get; init; }
}
