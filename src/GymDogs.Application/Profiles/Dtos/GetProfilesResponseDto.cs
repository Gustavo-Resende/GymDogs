namespace GymDogs.Application.Profiles.Dtos;

/// <summary>
/// DTO de resposta para listas de perfis com informação sobre resultados vazios
/// </summary>
public record GetProfilesResponseDto
{
    /// <summary>
    /// Lista de perfis encontrados
    /// </summary>
    public IEnumerable<GetProfileDto> Profiles { get; init; } = Enumerable.Empty<GetProfileDto>();

    /// <summary>
    /// Indica se a lista está vazia
    /// </summary>
    public bool IsEmpty => !Profiles.Any();

    /// <summary>
    /// Mensagem informativa quando não há perfis cadastrados
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Total de perfis encontrados
    /// </summary>
    public int TotalCount => Profiles.Count();
}
