using System.Runtime.CompilerServices;

namespace Factories.WebApi.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>>? GetAllAsync(CancellationToken token);
        T? Get(int id);
        void Create(T item);
        void Update(int id, T facility);
        void Delete(int id);

        Task SaveAsync();
    }
}
