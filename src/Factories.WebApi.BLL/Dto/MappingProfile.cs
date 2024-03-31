using AutoMapper;
using Factories.WebApi.DAL.Entities;

namespace Factories.WebApi.BLL.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Factory, FactoryDto>();
            CreateMap<FactoryDto, Factory>();

            CreateMap<Unit, UnitDto>();
            CreateMap<UnitDto, Unit>();

            CreateMap<Tank, TankDto>();
            CreateMap<TankDto, Tank>();
        }
    }
}
