using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

public class SearchExercisesByNameSpec : Specification<Exercise>
{
    public SearchExercisesByNameSpec(string searchTerm)
    {
        // Busca case-insensitive usando ToLower() em ambos os lados
        // EF Core traduz isso para uma query case-insensitive no PostgreSQL
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        Query.Where(e => e.Name.ToLower().Contains(lowerSearchTerm))
             .AsNoTracking() // Otimização: não rastreia entidades para queries de leitura
             .OrderBy(e => e.Name);
    }
}
