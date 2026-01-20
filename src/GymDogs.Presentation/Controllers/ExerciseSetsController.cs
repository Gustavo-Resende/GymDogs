using GymDogs.Application.ExerciseSets.Commands;
using GymDogs.Application.ExerciseSets.Dtos;
using GymDogs.Application.ExerciseSets.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento de séries de exercícios
/// </summary>
[ApiController]
[Route("api/folder-exercises/{folderExerciseId}/sets")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ExerciseSetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExerciseSetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adiciona uma série a um exercício de pasta
    /// </summary>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="request">Dados da série</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Série criada</returns>
    /// <response code="201">Série adicionada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateExerciseSetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateExerciseSetDto>> AddExerciseSet(
        [FromRoute] Guid folderExerciseId,
        [FromBody] AddExerciseSetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddExerciseSetCommand(
            folderExerciseId,
            request.SetNumber,
            request.Reps,
            request.Weight
        );
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista todas as séries de um exercício de pasta
    /// </summary>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de séries</returns>
    /// <response code="200">Lista de séries retornada com sucesso</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseSetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetExerciseSetDto>>> GetExerciseSetsByFolderExerciseId(
        [FromRoute] Guid folderExerciseId,
        CancellationToken cancellationToken)
    {
        var query = new GetExerciseSetsByFolderExerciseIdQuery(folderExerciseId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém uma série pelo ID
    /// </summary>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="setId">ID da série</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados da série</returns>
    /// <response code="200">Série encontrada</response>
    /// <response code="404">Série não encontrada</response>
    [HttpGet("{setId}")]
    [ProducesResponseType(typeof(GetExerciseSetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetExerciseSetDto>> GetExerciseSetById(
        [FromRoute] Guid folderExerciseId,
        [FromRoute] Guid setId,
        CancellationToken cancellationToken)
    {
        var query = new GetExerciseSetByIdQuery(setId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza uma série de exercício
    /// </summary>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="setId">ID da série</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Série atualizada</returns>
    /// <response code="200">Série atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Série não encontrada</response>
    [HttpPut("{setId}")]
    [ProducesResponseType(typeof(GetExerciseSetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetExerciseSetDto>> UpdateExerciseSet(
        [FromRoute] Guid folderExerciseId,
        [FromRoute] Guid setId,
        [FromBody] UpdateExerciseSetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExerciseSetCommand(setId, request.Reps, request.Weight);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deleta uma série de exercício
    /// </summary>
    /// <param name="folderExerciseId">ID do exercício na pasta</param>
    /// <param name="setId">ID da série</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Série deletada com sucesso</response>
    /// <response code="404">Série não encontrada</response>
    [HttpDelete("{setId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteExerciseSet(
        [FromRoute] Guid folderExerciseId,
        [FromRoute] Guid setId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExerciseSetCommand(setId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO para criação de série de exercício
/// </summary>
public record AddExerciseSetRequest
{
    /// <summary>
    /// Número da série (opcional, será auto-incrementado se não fornecido)
    /// </summary>
    /// <example>1</example>
    public int? SetNumber { get; init; }

    /// <summary>
    /// Número de repetições (entre 1 e 1000)
    /// </summary>
    /// <example>12</example>
    public int Reps { get; init; }

    /// <summary>
    /// Peso utilizado em kg (entre 0 e 10000)
    /// </summary>
    /// <example>80.5</example>
    public decimal Weight { get; init; }
}

/// <summary>
/// Request DTO para atualização de série de exercício
/// </summary>
public record UpdateExerciseSetRequest
{
    /// <summary>
    /// Novo número de repetições (opcional)
    /// </summary>
    /// <example>15</example>
    public int? Reps { get; init; }

    /// <summary>
    /// Novo peso utilizado em kg (opcional)
    /// </summary>
    /// <example>85.0</example>
    public decimal? Weight { get; init; }
}
