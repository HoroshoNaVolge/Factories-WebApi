using Factories.WebApi.DAL.EF.DbConfiguration;
using Factories.WebApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;

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

            modelBuilder.ApplyConfiguration(new FactoryConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new TankConfiguration());

            modelBuilder.UseLowerCaseTablesAndColumnsNames();
        }
    }
}
