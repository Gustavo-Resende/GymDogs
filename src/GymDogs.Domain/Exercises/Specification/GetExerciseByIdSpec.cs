using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

public class GetExerciseByIdSpec : Specification<Exercise>
{
    public GetExerciseByIdSpec(Guid id)
    {
        Query.Where(e => e.Id == id)
             .AsNoTracking(); // Optimization: does not track entities for read-only queries
    }
}
