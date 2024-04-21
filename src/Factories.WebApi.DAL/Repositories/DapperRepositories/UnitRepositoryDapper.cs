using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace Factories.WebApi.DAL.Repositories.DapperRepositories
{
    public class UnitRepositoryDapper(IRepository<Factory> factoriesRepository, IConfiguration configuration, IDistributedCache cache) : IRepository<Unit>
    {
        private readonly IRepository<Factory> factoriesRepository = factoriesRepository;
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection must be set up");
        private readonly IDistributedCache cache = cache;

        public async Task CreateAsync(Unit unit)
        {
            await cache.RemoveAsync("units_all");
            _ = factoriesRepository.Get(unit.FactoryId) ?? throw new ArgumentException($"Invalid factory id {unit.FactoryId}");

            using var connection = CreateConnection();
            var sql = "INSERT INTO Units (Name, Description, FactoryId) VALUES (@Name, @Description, @FactoryId)";
            await connection.ExecuteAsync(sql, unit);
        }
        public async Task DeleteAsync(int id)
        {
            await cache.RemoveAsync("units_all");

            using var connection = CreateConnection();
            var sql = "DELETE FROM Units WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public Unit? Get(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                      SELECT u.*, f.* FROM Units u
                      LEFT JOIN Factories f ON u.FactoryId = f.Id
                      WHERE u.Id = @Id;";

            var unitWithFactory = connection.Query<Unit, Factory, Unit>(
                sql,
                (unit, factory) =>
                {
                    unit.Factory = factory;
                    return unit;
                },
                new { Id = id },
                splitOn: "Id"
            ).FirstOrDefault();

            return unitWithFactory;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync(CancellationToken token)
        {
            var cacheKey = "units_all";
            var cachedData = await cache.GetStringAsync(cacheKey, token);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<Unit>>(cachedData) ?? throw new InvalidOperationException("JSON deserialization error"); ;
            }


            using var connection = CreateConnection();
            var sql = @"
                      SELECT u.*, f.* FROM Units u
                      LEFT JOIN Factories f ON u.FactoryId = f.Id;";

            var unitsDict = new Dictionary<int, Unit>();

            var units = await connection.QueryAsync<Unit, Factory, Unit>(
                sql,
                (unit, factory) =>
                {

                    if (!unitsDict.TryGetValue(unit.Id, out Unit? unitEntry))
                    {
                        unitEntry = unit;
                        unitsDict.Add(unitEntry.Id, unitEntry);
                    }

                    unitEntry.Factory = factory ?? unitEntry.Factory;
                    return unitEntry;
                },
                splitOn: "Id"
            );

            var serializedData = JsonSerializer.Serialize(unitsDict.Values);

            await cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            }, token);

            return unitsDict.Values;
        }

        public async Task UpdateAsync(int id, Unit unit)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Units SET Name = @Name, Description = @Description, FactoryId = @FactoryId WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { unit.Name, unit.Description, unit.FactoryId, Id = id });

            if (affectedRows == 0)
                throw new InvalidOperationException("Unit not found or no changes were made.");

            await cache.RemoveAsync("units_all");
        }

        private NpgsqlConnection CreateConnection() => new(connectionString);
    }
}
