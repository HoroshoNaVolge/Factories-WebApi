using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.DAL.Repositories
{
    public class TankRepository(FacilitiesDbContext db) : IRepository<Tank>, IDisposable
    {
        private readonly FacilitiesDbContext db = db;
        private bool disposed = false;

        public async Task CreateAsync(Tank item) => await db.Tanks.AddAsync(item);

        public void Delete(int id)
        {
            Tank? item = db.Tanks.Find(id);
            if (item != null)
                db.Tanks.Remove(item);
        }

        public IEnumerable<Tank> Find(Func<Tank, bool> predicate) => db.Tanks.Where(predicate).ToList();

        public Tank? Get(int id) => db.Tanks
                                .Include(t => t.Unit)
                                .FirstOrDefault(t => t.Id == id);

        public async Task<IEnumerable<Tank>>? GetAllAsync(CancellationToken token) =>
            await db.Tanks.Include(t => t.Unit)
            .ToListAsync(token);

        public void Update(int id, Tank tank)
        {
            var existingTank = db.Tanks.Find(id) ?? throw new InvalidOperationException("Tank not found");

            if (existingTank.Id != tank.Id)
                return;
            db.Entry(existingTank).CurrentValues.SetValues(tank);
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