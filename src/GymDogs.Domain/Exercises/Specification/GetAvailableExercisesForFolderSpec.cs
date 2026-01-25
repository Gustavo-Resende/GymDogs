using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

/// <summary>
/// Specification para buscar exercícios disponíveis (não adicionados) em uma pasta de treino.
/// Exclui exercícios que já estão na pasta especificada.
/// </summary>
public class GetAvailableExercisesForFolderSpec : Specification<Exercise>
{
    public GetAvailableExercisesForFolderSpec(Guid workoutFolderId)
    {
        // Busca exercícios que NÃO estão na pasta especificada
        // Usa subquery para verificar se o exercício já está na pasta
        Query.Where(e => !e.FolderExercises.Any(fe => fe.WorkoutFolderId == workoutFolderId))
             .AsNoTracking() // Otimização: não rastreia entidades para queries de leitura
             .OrderBy(e => e.Name);
    }
}
