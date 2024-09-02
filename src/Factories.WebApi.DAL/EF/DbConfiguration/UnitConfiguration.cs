using Factories.WebApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factories.WebApi.DAL.EF.DbConfiguration
{
    internal class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.HasData(
                  new Unit { Id = 1, Name = "ГФУ-2", Description = "Газофракционирующая установка", FactoryId = 1 },
                  new Unit { Id = 2, Name = "АВТ-6", Description = "Атмосферно-вакуумная трубчатка", FactoryId = 1 },
                  new Unit { Id = 3, Name = "АВТ-10", Description = "Атмосферно - вакуумная трубчатка", FactoryId = 2 });
        }
    }
}
