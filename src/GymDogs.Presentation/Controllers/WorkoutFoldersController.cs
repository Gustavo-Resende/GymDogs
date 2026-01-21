using GymDogs.Application.WorkoutFolders.Commands;
using GymDogs.Application.WorkoutFolders.Dtos;
using GymDogs.Application.WorkoutFolders.Queries;
using GymDogs.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GymDogs.Presentation.Controllers;

/// <summary>
/// Controller para gerenciamento de pastas de treino
/// </summary>
[ApiController]
[Route("api/profiles/{profileId}/workout-folders")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Authorize]
public class WorkoutFoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkoutFoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria uma nova pasta de treino
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="request">Dados da pasta de treino</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pasta de treino criada</returns>
    /// <response code="201">Pasta de treino criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateWorkoutFolderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CreateWorkoutFolderDto>> CreateWorkoutFolder(
        [FromRoute] Guid profileId,
        [FromBody] CreateWorkoutFolderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkoutFolderCommand(
            profileId,
            request.Name,
            request.Description,
            request.Order
        );
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Lista todas as pastas de treino de um perfil
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de pastas de treino</returns>
    /// <response code="200">Lista de pastas retornada com sucesso</response>
    /// <response code="404">Perfil não encontrado</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetWorkoutFolderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetWorkoutFolderDto>>> GetWorkoutFoldersByProfileId(
        [FromRoute] Guid profileId,
        CancellationToken cancellationToken)
    {
        var query = new GetWorkoutFoldersByProfileIdQuery(profileId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Obtém uma pasta de treino pelo ID
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="folderId">ID da pasta de treino</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados da pasta de treino</returns>
    /// <response code="200">Pasta de treino encontrada</response>
    /// <response code="404">Pasta de treino não encontrada</response>
    [HttpGet("{folderId}")]
    [ProducesResponseType(typeof(GetWorkoutFolderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetWorkoutFolderDto>> GetWorkoutFolderById(
        [FromRoute] Guid profileId,
        [FromRoute] Guid folderId,
        CancellationToken cancellationToken)
    {
        var query = new GetWorkoutFolderByIdQuery(folderId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza uma pasta de treino
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="folderId">ID da pasta de treino</param>
    /// <param name="request">Dados a serem atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pasta de treino atualizada</returns>
    /// <response code="200">Pasta de treino atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Pasta de treino não encontrada</response>
    [HttpPut("{folderId}")]
    [ProducesResponseType(typeof(GetWorkoutFolderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetWorkoutFolderDto>> UpdateWorkoutFolder(
        [FromRoute] Guid profileId,
        [FromRoute] Guid folderId,
        [FromBody] UpdateWorkoutFolderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWorkoutFolderCommand(folderId, request.Name, request.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Atualiza a ordem de uma pasta de treino
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="folderId">ID da pasta de treino</param>
    /// <param name="request">Nova ordem</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pasta de treino atualizada</returns>
    /// <response code="200">Ordem atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Pasta de treino não encontrada</response>
    [HttpPut("{folderId}/order")]
    [ProducesResponseType(typeof(GetWorkoutFolderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetWorkoutFolderDto>> UpdateWorkoutFolderOrder(
        [FromRoute] Guid profileId,
        [FromRoute] Guid folderId,
        [FromBody] UpdateWorkoutFolderOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWorkoutFolderOrderCommand(folderId, request.Order);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deleta uma pasta de treino
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="folderId">ID da pasta de treino</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="204">Pasta de treino deletada com sucesso</response>
    /// <response code="404">Pasta de treino não encontrada</response>
    [HttpDelete("{folderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteWorkoutFolder(
        [FromRoute] Guid profileId,
        [FromRoute] Guid folderId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWorkoutFolderCommand(folderId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO para criação de pasta de treino
/// </summary>
public record CreateWorkoutFolderRequest
{
    /// <summary>
    /// Nome da pasta de treino (máximo 200 caracteres)
    /// </summary>
    /// <example>Costas</example>
    [Required(ErrorMessage = "Name é obrigatório")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name deve ter entre 1 e 200 caracteres")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Descrição da pasta de treino (opcional)
    /// </summary>
    /// <example>Treino focado em desenvolvimento das costas</example>
    [StringLength(1000, ErrorMessage = "Description deve ter no máximo 1000 caracteres")]
    public string? Description { get; init; }

    /// <summary>
    /// Ordem de exibição da pasta (padrão: 0)
    /// </summary>
    /// <example>1</example>
    [Range(0, int.MaxValue, ErrorMessage = "Order deve ser maior ou igual a 0")]
    public int Order { get; init; } = 0;
}

/// <summary>
/// Request DTO para atualização de pasta de treino
/// </summary>
public record UpdateWorkoutFolderRequest
{
    /// <summary>
    /// Novo nome da pasta de treino
    /// </summary>
    /// <example>Costas e Bíceps</example>
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name deve ter entre 1 e 200 caracteres")]
    public string? Name { get; init; }

    /// <summary>
    /// Nova descrição da pasta de treino
    /// </summary>
    /// <example>Treino completo de costas e bíceps</example>
    [StringLength(1000, ErrorMessage = "Description deve ter no máximo 1000 caracteres")]
    public string? Description { get; init; }
}

/// <summary>
/// Request DTO para atualização de ordem da pasta de treino
/// </summary>
public record UpdateWorkoutFolderOrderRequest
{
    /// <summary>
    /// Nova ordem de exibição
    /// </summary>
    /// <example>2</example>
    [Required(ErrorMessage = "Order é obrigatório")]
    [Range(0, int.MaxValue, ErrorMessage = "Order deve ser maior ou igual a 0")]
    public int Order { get; init; }
}
