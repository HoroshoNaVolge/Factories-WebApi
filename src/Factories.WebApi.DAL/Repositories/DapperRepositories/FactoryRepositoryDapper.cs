using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Factories.WebApi.DAL.Repositories.DapperRepositories
{
    public class FactoryRepositoryDapper(IConfiguration configuration, IDistributedCache cache) : IRepository<Factory>
    {
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection must be set up");
        private readonly IDistributedCache cache = cache;

        public async Task CreateAsync(Factory factory)
        {
            await cache.RemoveAsync("factories_all");

            using var connection = CreateConnection();
            var sql = "INSERT INTO Factories (Name, Description) VALUES (@Name, @Description)";
            await connection.ExecuteAsync(sql, factory);
        }
        public async Task DeleteAsync(int id)
        {
            await cache.RemoveAsync("factories_all");

            using var connection = CreateConnection();
            var sql = "DELETE FROM Factories WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public Factory? Get(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Factories WHERE Id=@Id";

            return connection.QueryFirstOrDefault<Factory>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Factory>> GetAllAsync(CancellationToken token)
        {
            var cacheKey = "factories_all";
            var cachedData = await cache.GetStringAsync(cacheKey, token);

            if (!string.IsNullOrEmpty(cachedData))
                return JsonSerializer.Deserialize<List<Factory>>(cachedData) ?? throw new InvalidOperationException("JSON deserialization error");

            using var connection = CreateConnection();
            var sql = "SELECT * FROM Factories";

            var factories = await connection.QueryAsync<Factory>(sql, token);

            var serializedData = JsonSerializer.Serialize(factories);

            await cache.SetStringAsync(cacheKey, serializedData, token);

            return factories;
        }

        public async Task UpdateAsync(int id, Factory factory)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Factories SET Name = @Name, Description = @Description WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { factory.Name, factory.Description, Id = id });

            if (affectedRows == 0)
                throw new InvalidOperationException("Factory not found or no changes were made.");

            await cache.RemoveAsync("factories_all");
        }

        private NpgsqlConnection CreateConnection() => new(connectionString);
    }
}
