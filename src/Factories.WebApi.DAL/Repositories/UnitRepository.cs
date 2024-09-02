using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.DAL.Repositories
{
    public class UnitRepository(FacilitiesDbContext db, IRepository<Factory> factoriesRepository) : IRepository<Unit>
    {
        private readonly IRepository<Factory> factoriesRepository = factoriesRepository;
        private readonly FacilitiesDbContext db = db;

        public async Task CreateAsync(Unit unit)
        {
            unit.Factory = factoriesRepository.Get(unit.FactoryId) ?? throw new ArgumentException($"Invalid factory id {unit.FactoryId}");

            db.Units.Add(unit);

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Unit? item = db.Units.Find(id);
            if (item != null)
                db.Units.Remove(item);

            await db.SaveChangesAsync();
        }
        public Unit? Get(int id) => db.Units.Include(u => u.Factory)
                                .FirstOrDefault(u => u.Id == id);

        public async Task<IEnumerable<Unit>> GetAllAsync(CancellationToken token) =>
            await db.Units.Include(u => u.Factory).ToListAsync(token);

        public async Task<Unit?> GetAsync(int id, CancellationToken token)
        {
            if (db is null || db.Units is null)
                throw new InvalidOperationException("Database or table not initialized correctly");
            return await db.Units.FindAsync([id, token], cancellationToken: token);
        }

        public async Task UpdateAsync(int id, Unit unit)
        {
            Unit? existingUnit = db.Units.Find(id) ?? throw new InvalidOperationException("Unit not found");

            unit.Factory = factoriesRepository.Get(unit.FactoryId) ?? throw new ArgumentException($"Invalid factory id {unit.FactoryId}");

            db.Entry(existingUnit).State = EntityState.Modified;

            existingUnit.Name = unit.Name;
            existingUnit.Description = unit.Description;
            existingUnit.FactoryId = unit.FactoryId;

            await db.SaveChangesAsync();
        }
    }
}
