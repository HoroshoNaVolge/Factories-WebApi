using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Factories.WebApi.DAL.Repositories;
using Serilog;

namespace Factories.WebApi.BLL.Services
{
    public class WorkerService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

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

            var random = new Random();

            foreach (var tank in tanks)
            {
                // Генерация случайного числа в пределах от -0.1 до 0.1
                var randomChange = (random.NextDouble() - 0.5) * 0.2;

                var changeValue = tank.Volume * randomChange;

                tank.Volume += changeValue;

                if (tank.Volume > tank.MaxVolume)
                {
                    Log.Error($"Превышение максимального объёма резервуара: {tank.Name} Volume: {tank.Volume} MaxVolume: {tank.MaxVolume} ");
                    tank.Volume = tank.MaxVolume;
                }
                //else
                //    Log.Information($"Изменен объём резервуара {tank.Name} на {changeValue} до {tank.Volume} MaxVolume: {tank.MaxVolume}");
            }

        }
    }
}
