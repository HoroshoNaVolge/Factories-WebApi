using AutoMapper;
using Factories.WebApi.BLL.Dto;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Factories.WebApi.BLL.Controllers
{
    [ApiController]
    [Route("api/tank")]
    [Authorize]
    public class TankController(IRepository<Tank> tanksRepository, IMapper mapper) : ControllerBase
    {
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IRepository<Tank> tanksRepository = tanksRepository ?? throw new ArgumentNullException(nameof(tanksRepository));

        [HttpGet(template: "all")]
        public async Task<ActionResult<IReadOnlyCollection<TankDto>>> GetTanks(CancellationToken token)
        {
            var tanks = await (tanksRepository.GetAllAsync(token) ?? throw new NullReferenceException("UoW почему-то ноль или репо ноль"));

            var tankDtos = mapper.Map<IReadOnlyCollection<TankDto>>(tanks);

            return Ok(tankDtos);
        }

        [HttpGet(template: "{id}")]
        public IActionResult GetTank(int id)
        {
            var tank = tanksRepository.Get(id);

            if (tank == null) { return NotFound(); }

            var tankDto = mapper.Map<TankDto>(tank);

            return Ok(tankDto);
        }

        [Authorize(Policy = "TankOperatorPolicy")]
        [HttpPost]
        public IActionResult CreateTank(Tank tank)
        {
            tanksRepository.Create(tank);

            tanksRepository.Save();

            return Ok();
        }

        [Authorize(Policy = "TankOperatorPolicy")]
        [HttpPut]
        public IActionResult UpdateTank(int id, Tank tank)
        {
            tanksRepository.Update(id, tank);

            tanksRepository.Save();

            return Ok();
        }

        [Authorize(Policy = "TankOperatorPolicy")]
        [HttpDelete]
        public IActionResult DeleteTank(int id)
        {
            tanksRepository.Delete(id);

            tanksRepository.Save();

            return Ok();
        }
    }
}