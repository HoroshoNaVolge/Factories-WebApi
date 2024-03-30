﻿namespace Factories.WebApi.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>>? GetAllAsync(CancellationToken token);
        T? Get(int id);
        IEnumerable<T>? Find(Func<T, Boolean> predicate);
        Task CreateAsync(T item);
        void Update(int id, T facility);
        void Delete(int id);

        Task SaveAsync();
    }
}