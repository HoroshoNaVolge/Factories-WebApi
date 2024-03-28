using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.DAL.Repositories
{
    public class FactoryRepository(FacilitiesDbContext db) : IRepository<Factory>
    {
        private readonly FacilitiesDbContext db = db;
        private bool disposed = false;

        public async Task CreateAsync(Factory item) => await db.Factories.AddAsync(item);

        public void Delete(int id)
        {
            Factory? item = db.Factories.Find(id);
            if (item != null)
                db.Factories.Remove(item);
        }

        public IEnumerable<Factory> Find(Func<Factory, bool> predicate) => db.Factories.Where(predicate).ToList();

        public Factory? Get(int id) => db.Factories.Find(id);

        public async Task<IEnumerable<Factory>>? GetAllAsync(CancellationToken token) =>
            await db.Factories.ToListAsync(token);

        public void Update(int id, Factory factoryToUpdate)
        {
            var existingFactory = db.Factories.Find(id) ?? throw new InvalidOperationException("Factory not found");

            db.Entry(existingFactory).CurrentValues.SetValues(factoryToUpdate);
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
