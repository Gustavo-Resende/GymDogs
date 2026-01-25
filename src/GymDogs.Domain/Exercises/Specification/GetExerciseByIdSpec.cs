using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

public class GetExerciseByIdSpec : Specification<Exercise>
{
    public GetExerciseByIdSpec(Guid id)
    {
        Query.Where(e => e.Id == id)
             .AsNoTracking(); // Otimização: não rastreia entidades para queries de leitura
    }
}
