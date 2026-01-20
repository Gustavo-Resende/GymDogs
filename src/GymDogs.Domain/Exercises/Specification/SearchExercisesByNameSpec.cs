using Ardalis.Specification;

namespace GymDogs.Domain.Exercises.Specification;

public class SearchExercisesByNameSpec : Specification<Exercise>
{
    public SearchExercisesByNameSpec(string searchTerm)
    {
        Query.Where(e => e.Name.Contains(searchTerm))
             .OrderBy(e => e.Name);
    }
}
