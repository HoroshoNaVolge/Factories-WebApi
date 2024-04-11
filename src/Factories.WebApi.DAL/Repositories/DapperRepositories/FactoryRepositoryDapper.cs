using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Factories.WebApi.DAL.Repositories.DapperRepositories
{
    public class FactoryRepositoryDapper(IConfiguration configuration) : IRepository<Factory>
    {
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection must be set up");
        public async Task CreateAsync(Factory factory)
        {
            using var connection = CreateConnection();
            var sql = "INSERT INTO Factories (Name, Description) VALUES (@Name, @Description)";
            await connection.ExecuteAsync(sql, factory);
        }
        public async Task DeleteAsync(int id)
        {
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
            using var connection = CreateConnection();
            var sql = "SELECT * FROM Factories";

            return await connection.QueryAsync<Factory>(sql, token);
        }

        public async Task UpdateAsync(int id, Factory factory)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Factories SET Name = @Name, Description = @Description WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { factory.Name, factory.Description, Id = id });

            if (affectedRows == 0)
                throw new InvalidOperationException("Factory not found or no changes were made.");
        }

        private NpgsqlConnection CreateConnection() => new(connectionString);
    }
}
