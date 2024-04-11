using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Factories.WebApi.DAL.Repositories
{
    public class FactoryRepository(FacilitiesDbContext db) : IRepository<Factory>
    {
        private readonly FacilitiesDbContext db = db;

        public async Task CreateAsync(Factory item)
        {
            db.Factories.Add(item);

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Factory? item = db.Factories.Find(id);

            if (item != null)
                db.Factories.Remove(item);

            await db.SaveChangesAsync();
        }

        public Factory? Get(int id) => db.Factories.Find(id);

        public async Task<IEnumerable<Factory>>? GetAllAsync(CancellationToken token) =>
            await db.Factories.ToListAsync(token);

        public async Task UpdateAsync(int id, Factory factoryToUpdate)
        {
            var existingFactory = db.Factories.Find(id) ?? throw new InvalidOperationException("Factory not found");

            db.Entry(existingFactory).State = EntityState.Modified;

            existingFactory.Name = factoryToUpdate.Name;
            existingFactory.Description = factoryToUpdate.Description;

            await db.SaveChangesAsync();
        }
    }
}
