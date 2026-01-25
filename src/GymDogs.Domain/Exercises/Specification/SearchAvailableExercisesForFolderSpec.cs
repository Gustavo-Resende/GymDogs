using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

/// <summary>
/// Specification para buscar exercícios disponíveis (não adicionados) em uma pasta de treino,
/// filtrando por nome (busca case-insensitive).
/// Exclui exercícios que já estão na pasta especificada.
/// </summary>
public class SearchAvailableExercisesForFolderSpec : Specification<Exercise>
{
    public SearchAvailableExercisesForFolderSpec(Guid workoutFolderId, string searchTerm)
    {
        // Busca exercícios que:
        // 1. NÃO estão na pasta especificada
        // 2. O nome contém o termo de busca (case-insensitive)
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        Query.Where(e => !e.FolderExercises.Any(fe => fe.WorkoutFolderId == workoutFolderId) &&
                        e.Name.ToLower().Contains(lowerSearchTerm))
             .AsNoTracking() // Otimização: não rastreia entidades para queries de leitura
             .OrderBy(e => e.Name);
    }
}
