using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;


namespace Factories.WebApi.DAL.Repositories.DapperRepositories
{
    public class TankRepositoryDapper(IRepository<Unit> unitsRepository, IConfiguration configuration) : IRepository<Tank>
    {
        private readonly IRepository<Unit> unitsRepository = unitsRepository;
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection must be set up");
        public void Create(Tank tank)
        {
            _ = unitsRepository.Get(tank.UnitId) ?? throw new ArgumentException($"Invalid unit id {tank.UnitId}");

            using var connection = CreateConnection();
            var sql = "INSERT INTO Tanks (Name, Description, Volume, MaxVolume, UnitId) VALUES (@Name, @Description, @Volume, @MaxVolume, @UnitId)";
            connection.Execute(sql, tank);
        }
        public void Delete(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Tanks WHERE Id = @Id";
            connection.Execute(sql, new { Id = id });
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

            return tankDict.Values;
        }

        public void Update(int id, Tank tank)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Tanks SET Name = @Name, Description = @Description, Volume = @Volume, MaxVolume = @MaxVolume, UnitId = @UnitId WHERE Id = @Id";
            var affectedRows = connection.Execute(sql, new { tank.Name, tank.Description, tank.Volume, tank.MaxVolume, tank.UnitId, Id = id });

            if (affectedRows == 0)
                throw new InvalidOperationException("Tank not found or no changes were made.");
        }

        public async Task SaveAsync()
        {
            await Task.Delay(0);
        }

        private NpgsqlConnection CreateConnection() => new(connectionString);
    }
}
