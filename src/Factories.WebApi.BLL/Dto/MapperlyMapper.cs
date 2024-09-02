using Factories.WebApi.DAL.Entities;
using Riok.Mapperly.Abstractions;

namespace Factories.WebApi.BLL.Dto
{
    [Mapper]
    public partial class MapperlyMapper
    {
        [MapperIgnoreSource(nameof(Tank.Unit))]
        public partial TankDto TankToTankDto(Tank tank);

        [MapperIgnoreTarget(nameof(Tank.Unit))]
        public partial Tank TankDtoToTank(TankDto tankDto);

        [MapperIgnoreSource(nameof(Unit.Factory))]
        public partial UnitDto UnitToUnitDto(Unit unit);

        [MapperIgnoreTarget(nameof(Unit.Factory))]
        public partial Unit UnitDtoToUnit(UnitDto unitDto);
    }
}
