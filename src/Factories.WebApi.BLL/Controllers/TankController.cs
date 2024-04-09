﻿using Riok.Mapperly;
using Factories.WebApi.BLL.Dto;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Factories.WebApi.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Factories.WebApi.BLL.Controllers
{
    [ApiController]
    [Route("api/tank")]
    [Authorize(Policy = "AdminOrTankOperatorPolicy")]
    public class TankController(IRepository<Tank> tanksRepository, MapperlyMapper mapper) : ControllerBase
    {
        private readonly MapperlyMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IRepository<Tank> tanksRepository = tanksRepository ?? throw new ArgumentNullException(nameof(tanksRepository));

        [HttpGet(template: "all")]
        public async Task<ActionResult<IReadOnlyCollection<TankDto>>> GetTanks(CancellationToken token)
        {
            var tanks = await (tanksRepository.GetAllAsync(token) ?? throw new NullReferenceException("UoW почему-то ноль или репо ноль"));

            var tankDtos = tanks.Select(mapper.TankToTankDto).ToList();

            return Ok(tankDtos);
        }

        [HttpGet(template: "{id}")]
        public IActionResult GetTank(int id)
        {
            var tank = tanksRepository.Get(id);

            if (tank == null) { return NotFound(); }

            var tankDto = mapper.TankToTankDto(tank);

            return Ok(tankDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTankAsync(TankDto tankDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tank = mapper.TankDtoToTank(tankDto);

            tanksRepository.Create(tank);

            await tanksRepository.SaveAsync();

            return Ok($"Создан резервуар {tank.Name} на установке id {tank.UnitId}");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTankAsync(int id, TankDto tankDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tank = mapper.TankDtoToTank(tankDto);

            tanksRepository.Update(id, tank);

            await tanksRepository.SaveAsync();

            return Ok($"Обновлен {tank.Name}");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTankAsync(int id)
        {
            tanksRepository.Delete(id);

            await tanksRepository.SaveAsync();

            return Ok($"Удален резервуар под номером {id}");
        }
    }
}
