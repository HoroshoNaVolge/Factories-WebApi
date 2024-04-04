using Factories.WebApi.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Factories.WebApi.DAL.EF
{
    public class FacilitiesDbContext(DbContextOptions<FacilitiesDbContext> options) : DbContext(options)
    {
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Tank> Tanks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Коммен для себя: навигационные свойства не нужно явно добавлять при инициализации, EF сам свяжет по Id.
            modelBuilder.Entity<Factory>().HasData(
                new Factory { Id = 1, Name = "НПЗ№1", Description = "Первый нефтеперерабатывающий завод" },
                new Factory { Id = 2, Name = "НПЗ№2", Description = "Второй нефтеперерабатывающий завод" });

            modelBuilder.Entity<Unit>().HasData(
                new Unit { Id = 1, Name = "ГФУ-2", Description = "Газофракционирующая установка", FactoryId = 1 },
                new Unit { Id = 2, Name = "АВТ-6", Description = "Атмосферно-вакуумная трубчатка", FactoryId = 1 },
                new Unit { Id = 3, Name = "АВТ-10", Description = "Атмосферно - вакуумная трубчатка", FactoryId = 2 });

            modelBuilder.Entity<Tank>().HasData(
                new Tank { Id = 1, Name = "Резервуар 1", Description = "Надземный-вертикальный", UnitId = 1, Volume = 1500, MaxVolume = 2000 },
                new Tank { Id = 2, Name = "Резервуар 2", Description = "Надземный-горизонтальный", UnitId = 1, Volume = 2500, MaxVolume = 3000 },
                new Tank { Id = 3, Name = "Резервуар 3", Description = "Надземный-горизонтальный", UnitId = 2, Volume = 3000, MaxVolume = 3000 },
                new Tank { Id = 4, Name = "Резервуар 4", Description = "Надземный-вертикальный", UnitId = 2, Volume = 3000, MaxVolume = 3000 },
                new Tank { Id = 5, Name = "Резервуар 5", Description = "Подземный-двустенный", UnitId = 2, Volume = 4000, MaxVolume = 5000 },
                new Tank { Id = 6, Name = "Резервуар 6", Description = "Подводный", UnitId = 2, Volume = 500, MaxVolume = 500 });
        }
    }
}
