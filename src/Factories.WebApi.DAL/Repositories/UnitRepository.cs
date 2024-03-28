using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.DAL.Repositories
{
    public class UnitRepository(FacilitiesDbContext db) : IRepository<Unit>, IDisposable
    {
        private readonly FacilitiesDbContext db = db;
        private bool disposed = false;
        public async Task CreateAsync(Unit item) => await db.Units.AddAsync(item);

        public void Delete(int id)
        {
            Unit? item = db.Units.Find(id);
            if (item != null)
                db.Units.Remove(item);
        }

        public IEnumerable<Unit> Find(Func<Unit, bool> predicate) => db.Units.Where(predicate).ToList();

        public Unit? Get(int id) => db.Units.Include(u => u.Factory)
                                .FirstOrDefault(u => u.Id == id);

        public async Task<IEnumerable<Unit>> GetAllAsync(CancellationToken token) =>
            await db.Units.Include(u => u.Factory).ToListAsync(token);

        public void Update(int id, Unit unit)
        {
            Unit? existingUnit = db.Units.Find(id) ?? throw new InvalidOperationException("Unit not found");

            db.Entry(existingUnit).CurrentValues.SetValues(unit);
        }

        public async Task SaveAsync() => await db.SaveChangesAsync();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    db.Dispose();

                disposed = true;
            }
        }
    }
}