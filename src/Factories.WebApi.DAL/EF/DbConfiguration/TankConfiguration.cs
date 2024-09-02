using Factories.WebApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factories.WebApi.DAL.EF.DbConfiguration
{
    internal class TankConfiguration : IEntityTypeConfiguration<Tank>
    {
        public void Configure(EntityTypeBuilder<Tank> builder)
        {
            builder.HasData(
                new Tank { Id = 1, Name = "Резервуар 1", Description = "Надземный-вертикальный", UnitId = 1, Volume = 1500, MaxVolume = 2000 },
                new Tank { Id = 2, Name = "Резервуар 2", Description = "Надземный-горизонтальный", UnitId = 1, Volume = 2500, MaxVolume = 3000 },
                new Tank { Id = 3, Name = "Резервуар 3", Description = "Надземный-горизонтальный", UnitId = 2, Volume = 3000, MaxVolume = 3000 },
                new Tank { Id = 4, Name = "Резервуар 4", Description = "Надземный-вертикальный", UnitId = 2, Volume = 3000, MaxVolume = 3000 },
                new Tank { Id = 5, Name = "Резервуар 5", Description = "Подземный-двустенный", UnitId = 2, Volume = 4000, MaxVolume = 5000 },
                new Tank { Id = 6, Name = "Резервуар 6", Description = "Подводный", UnitId = 2, Volume = 500, MaxVolume = 500 });
        }
    }
}
