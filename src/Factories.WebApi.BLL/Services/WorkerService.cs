using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Serilog;

namespace Factories.WebApi.BLL.Services
{
    public class WorkerService(IServiceScopeFactory serviceScopeFactory, IRandomService randomService) : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;
        private readonly IRandomService randomService = randomService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var tankRepository = scope.ServiceProvider.GetRequiredService<IRepository<Tank>>();

                if (tankRepository is null)
                    return;

                await UpdateAllVolumesRandomlyAsync(tankRepository, stoppingToken);
                await tankRepository.SaveAsync();
                await Task.Delay(5000, stoppingToken);
            }
        }

        public async Task UpdateAllVolumesRandomlyAsync(IRepository<Tank> tanksRepository, CancellationToken stoppingToken)
        {
            var tanks = await tanksRepository.GetAllAsync(stoppingToken)!;

            double updatedVolume;
            double randomChange;

            foreach (var tank in tanks)
            {
                // Генерация случайного числа в пределах от -0.1 до 0.1
                randomChange = (randomService.NextDouble() - 0.5) * 0.2;

                updatedVolume = (double)(tank.Volume + tank.Volume * randomChange)!;

                if (updatedVolume > tank.MaxVolume)
                {
                    Log.Error($"Превышение максимального объёма резервуара: {tank.Name} Volume: {tank.Volume} MaxVolume: {tank.MaxVolume} ");
                    tank.Volume = tank.MaxVolume;
                }

                tank.Volume = updatedVolume;

                tanksRepository.Update(tank.Id, tank);
            }
        }
    }
}
