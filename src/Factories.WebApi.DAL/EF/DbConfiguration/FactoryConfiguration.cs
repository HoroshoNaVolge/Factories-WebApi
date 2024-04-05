using Factories.WebApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factories.WebApi.DAL.EF.DbConfiguration
{
    internal class FactoryConfiguration : IEntityTypeConfiguration<Factory>
    {
        public void Configure(EntityTypeBuilder<Factory> builder)
        {
            builder.HasData(
                  new Factory { Id = 1, Name = "НПЗ№1", Description = "Первый нефтеперерабатывающий завод" },
                  new Factory { Id = 2, Name = "НПЗ№2", Description = "Второй нефтеперерабатывающий завод" });
        }
    }
}
