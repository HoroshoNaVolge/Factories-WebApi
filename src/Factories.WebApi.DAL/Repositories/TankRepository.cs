﻿using Factories.WebApi.DAL.EF;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Factories.WebApi.DAL.Repositories
{
    public class TankRepository(FacilitiesDbContext db, IRepository<Unit> unitsRepository) : IRepository<Tank>
    {
        private readonly IRepository<Unit> unitsRepository = unitsRepository;
        private readonly FacilitiesDbContext db = db;

        public void Create(Tank tank)
        {
            tank.Unit = unitsRepository.Get(tank.UnitId) ?? throw new ArgumentException($"Invalid unit id {tank.UnitId}");

            db.Tanks.Add(tank);
        }
        public void Delete(int id)
        {
            Tank? item = db.Tanks.Find(id);
            if (item != null)
                db.Tanks.Remove(item);
        }

        public Tank? Get(int id) => db.Tanks
                                .Include(t => t.Unit)
                                .FirstOrDefault(t => t.Id == id);

        public async Task<IEnumerable<Tank>>? GetAllAsync(CancellationToken token) =>
            await db.Tanks.Include(t => t.Unit)
            .ToListAsync(token);

        public void Update(int id, Tank tank)
        {
            var existingTank = db.Tanks.Find(id) ?? throw new InvalidOperationException("Tank not found");
            tank.Unit = unitsRepository.Get(tank.UnitId) ?? throw new ArgumentException($"Invalid unit id {tank.UnitId}");

            db.Entry(existingTank).State = EntityState.Modified;

            existingTank.Name = tank.Name;
            existingTank.Volume = tank.Volume;
            existingTank.MaxVolume = tank.MaxVolume;
            existingTank.Description = tank.Description;
            existingTank.Name = tank.Name;
        }

        public async Task SaveAsync() => await db.SaveChangesAsync();
    }
}
