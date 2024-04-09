using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Factories.WebApi.DAL.Repositories.DapperRepositories
{
    public class FactoryRepositoryDapper(IConfiguration configuration) : IRepository<Factory>
    {
        private readonly string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection must be set up");
        public void Create(Factory factory)
        {
            using var connection = CreateConnection();
            var sql = "INSERT INTO Factories (Name, Description) VALUES (@Name, @Description)";
            connection.Execute(sql, factory);
        }
        public void Delete(int id)
        {
            using var connection = CreateConnection();
            var sql = "DELETE FROM Factories WHERE Id = @Id";
            connection.Execute(sql, new { Id = id });
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

        public void Update(int id, Factory factory)
        {
            using var connection = CreateConnection();
            var sql = "UPDATE Factories SET Name = @Name, Description = @Description WHERE Id = @Id";
            var affectedRows = connection.Execute(sql, new { factory.Name, factory.Description, Id = id });

            if (affectedRows == 0)
                throw new InvalidOperationException("Factory not found or no changes were made.");
        }

        public async Task SaveAsync()
        {
            await Task.Delay(0);
            // Насколько я понял, транзакции не нужны для простых операций СRUD, а Dapper сам сохраняет изменения в БД при запросах.
            // Так оставлять без реализации допустимо? Для EF сохранение изменений нормально вставлять сразу в сами методы CRUD? Тогда можно удалить SaveAsync из общего интерфейса
        }

        private NpgsqlConnection CreateConnection() => new(connectionString);
    }
}
