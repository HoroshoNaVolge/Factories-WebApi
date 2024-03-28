using AutoMapper;
using Factories.WebApi.BLL.Dto;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Factories.WebApi.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Factories.WebApi.BLL.Controllers
{
    [ApiController]
    [Route("api/unit")]
    [Authorize]
    public class UnitController(IRepository<Unit> unitsRepository, IMapper mapper) : ControllerBase
    {
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IRepository<Unit> unitsRepository = unitsRepository ?? throw new ArgumentNullException(nameof(unitsRepository));

        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyCollection<UnitDto>>> GetUnits(CancellationToken token)
        {
            var units = await (unitsRepository.GetAllAsync(token) ?? throw new NullReferenceException("UoW почему-то ноль или репо ноль"));

            return Ok(mapper.Map<IReadOnlyCollection<UnitDto>>(units));
        }

        [HttpGet("{id}")]
        public IActionResult GetUnit(int id)
        {
            var unit = unitsRepository.Get(id);

            if (unit == null)
                return NotFound();

            return Ok(mapper.Map<UnitDto>(unit));
        }

        [Authorize(Policy = "UnitOperatorPolicy")]
        [HttpPost]
        public IActionResult CreateUnit(Unit unit)
        {
            unitsRepository.Create(unit);
            unitsRepository.Save();

            return Ok();
        }

        [Authorize(Policy = "UnitOperatorPolicy")]
        [HttpPut]
        public IActionResult UpdateUnit(int id, Unit unit)
        {
            unitsRepository.Update(id, unit);
            unitsRepository.Save();

            return Ok();
        }

        [Authorize(Policy = "UnitOperatorPolicy")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUnit(int id)
        {
            unitsRepository.Delete(id);
            unitsRepository.Save();

            return Ok();
        }
    }
}