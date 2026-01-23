using Ardalis.Specification;

namespace GymDogs.Application.Interfaces
{
    /// <summary>
    /// Abstraction for a repository pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IRepositoryBase<T> where T : class
    {
    }
    
    /// <summary>
    /// Abstraction for a read-only repository pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class
    {
    }
}
