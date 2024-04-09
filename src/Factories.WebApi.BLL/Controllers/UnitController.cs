using Factories.WebApi.BLL.Dto;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Factories.WebApi.BLL.Controllers
{
    [ApiController]
    [Route("api/unit")]
    [Authorize(Policy = "AdminOrUnitOperatorPolicy")]
    public class UnitController(IRepository<Unit> unitsRepository, MapperlyMapper mapper) : ControllerBase
    {
        private readonly MapperlyMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IRepository<Unit> unitsRepository = unitsRepository ?? throw new ArgumentNullException(nameof(unitsRepository));

        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyCollection<UnitDto>>> GetUnits(CancellationToken token)
        {
            var units = await (unitsRepository.GetAllAsync(token) ?? throw new NullReferenceException("UoW почему-то ноль или репо ноль"));

            return Ok(units.Select(mapper.UnitToUnitDto).ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetUnit(int id)
        {
            var unit = unitsRepository.Get(id);

            if (unit == null)
                return NotFound();

            return Ok(mapper.UnitToUnitDto(unit));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUnitAsync(UnitDto unitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var unit = mapper.UnitDtoToUnit(unitDto);

            unitsRepository.Create(unit);

            await unitsRepository.SaveAsync();

            return Ok($"Добавлена установка {unit.Name} на завод id: {unit.FactoryId}");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUnit(int id, UnitDto unitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var unit = mapper.UnitDtoToUnit(unitDto);

            unitsRepository.Update(id, unit);

            await unitsRepository.SaveAsync();

            return Ok($"Установка {unit.Name} обновлена");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnitAsync(int id)
        {
            unitsRepository.Delete(id);

            await unitsRepository.SaveAsync();

            return Ok($"Установка по номеру {id} удалена");
        }
    }
}
