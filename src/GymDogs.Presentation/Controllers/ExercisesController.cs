using GymDogs.Application.Exercises.Commands;
using GymDogs.Application.Exercises.Dtos;
using GymDogs.Application.Exercises.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento do catálogo de exercícios
/// </summary>
[ApiController]
[Route("api/exercises")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ExercisesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExercisesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria um novo exercício no catálogo
    /// </summary>
    /// <param name="request">Dados do exercício</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Exercício criado</returns>
    /// <response code="201">Exercício criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateExerciseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateExerciseDto>> CreateExercise(
        [FromBody] CreateExerciseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateExerciseCommand(request.Name, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista todos os exercícios do catálogo
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de exercícios</returns>
    /// <response code="200">Lista de exercícios retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetExerciseDto>>> GetAllExercises(
        CancellationToken cancellationToken)
    {
        var query = new GetAllExercisesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Busca exercícios por nome
    /// </summary>
    /// <param name="searchTerm">Termo de busca</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de exercícios encontrados</returns>
    /// <response code="200">Lista de exercícios retornada com sucesso</response>
    /// <response code="400">Termo de busca inválido</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<GetExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GetExerciseDto>>> SearchExercisesByName(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchExercisesByNameQuery(searchTerm);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém um exercício pelo ID
    /// </summary>
    /// <param name="id">ID do exercício</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do exercício</returns>
    /// <response code="200">Exercício encontrado</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetExerciseDto>> GetExerciseById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetExerciseByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza um exercício
    /// </summary>
    /// <param name="id">ID do exercício</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Exercício atualizado</returns>
    /// <response code="200">Exercício atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GetExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetExerciseDto>> UpdateExercise(
        [FromRoute] Guid id,
        [FromBody] UpdateExerciseRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExerciseCommand(id, request.Name, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deleta um exercício do catálogo
    /// </summary>
    /// <param name="id">ID do exercício</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Exercício deletado com sucesso</response>
    /// <response code="404">Exercício não encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteExercise(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExerciseCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO para criação de exercício
/// </summary>
public record CreateExerciseRequest
{
    /// <summary>
    /// Nome do exercício (máximo 200 caracteres)
    /// </summary>
    /// <example>Supino Reto</example>
    [Required(ErrorMessage = "Name é obrigatório")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name deve ter entre 1 e 200 caracteres")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Descrição do exercício (opcional, máximo 1000 caracteres)
    /// </summary>
    /// <example>Exercício para desenvolvimento do peitoral</example>
    [StringLength(1000, ErrorMessage = "Description deve ter no máximo 1000 caracteres")]
    public string? Description { get; init; }
}

/// <summary>
/// Request DTO para atualização de exercício
/// </summary>
public record UpdateExerciseRequest
{
    /// <summary>
    /// Novo nome do exercício
    /// </summary>
    /// <example>Supino Reto com Barra</example>
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name deve ter entre 1 e 200 caracteres")]
    public string? Name { get; init; }

    /// <summary>
    /// Nova descrição do exercício
    /// </summary>
    /// <example>Exercício para desenvolvimento do peitoral usando barra</example>
    [StringLength(1000, ErrorMessage = "Description deve ter no máximo 1000 caracteres")]
    public string? Description { get; init; }
}
