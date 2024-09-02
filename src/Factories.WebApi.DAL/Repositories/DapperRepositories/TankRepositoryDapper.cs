using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace Factories.WebApi.DAL.Repositories.DapperRepositories
{
    public class TankRepositoryDapper(IRepository<Unit> unitsRepository, IConfiguration configuration, IDistributedCache cache) : IRepository<Tank>
    {
        private readonly IRepository<Unit> unitsRepository = unitsRepository;
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection must be set up");
        private readonly IDistributedCache cache = cache;

        public async Task CreateAsync(Tank tank)
        {
            _ = unitsRepository.Get(tank.UnitId) ?? throw new ArgumentException($"Invalid unit id {tank.UnitId}");

            await cache.RemoveAsync("tanks_all");

            using var connection = CreateConnection();
            var sql = "INSERT INTO Tanks (Name, Description, Volume, MaxVolume, UnitId) VALUES (@Name, @Description, @Volume, @MaxVolume, @UnitId)";
            await connection.ExecuteAsync(sql, tank);
        }
        public async Task DeleteAsync(int id)
        {
            await cache.RemoveAsync("tanks_all");

            using var connection = CreateConnection();
            var sql = "DELETE FROM Tanks WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public Tank? Get(int id)
        {
            using var connection = CreateConnection();
            var sql = @"
                      SELECT t.*, u.* FROM Tanks t
                      LEFT JOIN Units u ON t.UnitId = u.Id
                      WHERE t.Id = @Id;";

            var tankWithUnit = connection.Query<Tank, Unit, Tank>(
                sql,
                (tank, unit) =>
                {
                    tank.Unit = unit;
                    return tank;
                },
                new { Id = id },
                splitOn: "Id"
            ).FirstOrDefault();

            return tankWithUnit;
        }

        public async Task<IEnumerable<Tank>> GetAllAsync(CancellationToken token)
        {
            var cacheKey = "tanks_all";
            var cachedData = await cache.GetStringAsync(cacheKey, token);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<Tank>>(cachedData) ?? throw new InvalidOperationException("JSON deserialization error");
            }

            using var connection = CreateConnection();
            var sql = @"
                      SELECT t.*, u.* FROM Tanks t
                      LEFT JOIN Units u ON t.UnitId = u.Id;";

            var tankDict = new Dictionary<int, Tank>();

            var tanks = await connection.QueryAsync<Tank, Unit, Tank>(
                sql,
                (tank, unit) =>
                {

                    if (!tankDict.TryGetValue(tank.Id, out Tank? tankEntry))
                    {
                        tankEntry = tank;
                        tankDict.Add(tankEntry.Id, tankEntry);
                    }

                    tankEntry.Unit = unit ?? tankEntry.Unit;
                    return tankEntry;
                },
                splitOn: "Id"
            );

            var serializedData = JsonSerializer.Serialize(tankDict.Values);

            await cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) // кэш истекает через 30 минут
            }, token);


            return tankDict.Values;
        }

        public async Task<Tank?> GetAsync(int id, CancellationToken token)
        {
            var cacheKey = $"units_{id}";
            var cachedData = await cache.GetStringAsync(cacheKey, token);

            if (!string.IsNullOrEmpty(cachedData))
                return JsonSerializer.Deserialize<Tank>(cachedData) ?? throw new InvalidOperationException("JSON deserialization error"); ;

            using var connection = CreateConnection();
            var sql = @"
                      SELECT t.*, u.* FROM Tanks t
                      LEFT JOIN Units u ON t.UnitId= u.Id
                      WHERE t.Id = @Id;";

            var tankWithFactory = await connection.QueryAsync<Tank, Unit, Tank>(
                sql,
                (tank, unit) =>
                {
                    tank.Unit= unit;
                    return tank;
                },
                new { Id = id },
                splitOn: "Id"
            );

            var foundTank = tankWithFactory.FirstOrDefault();

            if (foundTank is not null)
            {
                var serializedData = JsonSerializer.Serialize(foundTank);

                await cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                }, token: token);
            }

            return foundTank;
        }

        public async Task UpdateAsync(int id, Tank tank)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Tanks SET Name = @Name, Description = @Description, Volume = @Volume, MaxVolume = @MaxVolume, UnitId = @UnitId WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { tank.Name, tank.Description, tank.Volume, tank.MaxVolume, tank.UnitId, Id = id });

            if (affectedRows == 0)
                throw new InvalidOperationException("Tank not found or no changes were made.");

            await cache.RemoveAsync("tanks_all");
        }

        private NpgsqlConnection CreateConnection() => new(connectionString);
    }
}
