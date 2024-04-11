using System.Runtime.CompilerServices;

namespace Factories.WebApi.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>>? GetAllAsync(CancellationToken token);
        T? Get(int id);
        Task CreateAsync(T item);
        Task UpdateAsync(int id, T facility);
        Task DeleteAsync(int id);
    }
}
