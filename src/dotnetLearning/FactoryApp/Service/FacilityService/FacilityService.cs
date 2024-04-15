using dotnetLearning.FactoryApp.Model;
using dotnetLearning.FactoryApp.Service.SerializationService;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace dotnetLearning.FactoryApp.Service.FacilityService
{
    public interface IFacilityService
    {
        public Task CreateOrUpdateDataAsync(SerializationServiceType serviceType, CancellationToken token);
        public Task GetDataAsync(SerializationServiceType serviceType, CancellationToken token);
        public Task AddDataAsync(SerializationServiceType serviceType, IFacility facility, CancellationToken token);
        public Task DeleteDataAsync(SerializationServiceType serviceType, IFacility facility, CancellationToken token);
        public Task UpdateDataAsync(SerializationServiceType serviceType, IFacility facility, CancellationToken token);

        /// <returns>Строка с количеством текущих объектов Factory, Unit,Tank</returns>
        public string? GetCurrentConfiguration();
        public string? GetTotalSummary();
        public string? GetTotalVolumeTanks();
        public string? GetTanksSummary();
        public string? GetFactoriesSummary();
        public string? GetUnitsSummary();

        public Unit? FindUnit(string tankName);
        public Factory? FindFactory(Unit unit);
        public Factory? FindFactory(string unitName);
        public IFacility? Search(string name);
    }

    public class FacilityService(ISerializationServiceFactory serviceFactory) : IFacilityService
    {
        private FacilitiesContainer? container = null;
        private readonly ISerializationServiceFactory serviceFactory = serviceFactory;

        private IList<Factory>? factories = null;
        private IList<Unit>? units = null;
        private IList<Tank>? tanks = null;

        public async Task CreateOrUpdateDataAsync(SerializationServiceType serviceType, CancellationToken token)
        {
            if (factories is null || tanks is null || units is null)
                return;

            var serializationService = serviceFactory.CreateService(serviceType);

            await serializationService!.CreateOrUpdateAllAsync(container ?? new(factories, units, tanks), token);
        }

        public async Task AddDataAsync(SerializationServiceType serviceType, IFacility facility, CancellationToken token)
        {
            if (facility is null) return;

            var serializationService = serviceFactory.CreateService(serviceType);

            await serializationService!.AddFacilityAsync(facility, token);
        }

        public async Task GetDataAsync(SerializationServiceType serviceType, CancellationToken token)
        {
            var serializationService = serviceFactory.CreateService(serviceType);

            if (container is null)
#nullable disable
                container = new(factories, units, tanks);
#nullable restore
            await serializationService.GetFacilitiesAsync(container, token);

            factories = container.Factories;
            units = container.Units;
            tanks = container.Tanks;
        }

        public async Task DeleteDataAsync(SerializationServiceType serviceType, IFacility facility, CancellationToken token)
        {
            if (facility is null) return;

            var serializationService = serviceFactory.CreateService(serviceType);

            await serializationService!.DeleteFacilityAsync(facility, token);
        }

        public async Task UpdateDataAsync(SerializationServiceType serviceType, IFacility facility, CancellationToken token)
        {
            if (facility is null) return;

            var serializationService = serviceFactory.CreateService(serviceType);

            await serializationService!.UpdateFacilityAsync(facility, token);
        }

        #region GetInfoAsString
        public string? GetFactoriesSummary() => factories != null ? string.Join(Environment.NewLine, factories) : null;

        public string? GetUnitsSummary() => units != null ? string.Join(Environment.NewLine, units) : null;

        public string? GetTanksSummary() => tanks != null ? string.Join(Environment.NewLine, tanks) : null;

        public string? GetCurrentConfiguration() { return $"Количество резервуаров: {tanks?.Count ?? 0}, установок: {units?.Count ?? 0}"; }

        public string? GetTotalSummary()
        {
            if (factories is null || units is null || tanks is null) return null;

            // Наверное не стоит кастить к обджекту, просто хотел сделал лакончино.
            var items = factories.Cast<object>().Concat(units).Concat(tanks);

            return $"По состоянию на {DateTime.Now} в работе:" + Environment.NewLine +
                     string.Join(Environment.NewLine, items.Select(item => item.ToString()));
        }

        public string? GetTotalVolumeTanks()
        {
            if (tanks != null)
                return $"Общая заполненность резервуаров, мт: {tanks?.Sum(tank => tank.Volume)}";
            return null;
        }
        #endregion

        #region Searches
        public Unit? FindUnit(string tankName) =>
            units?.FirstOrDefault(u => tanks?.Any(t => t.Name == tankName && t.UnitId == u.Id) ?? false);

        // Считаю нужным перегрузить на приём строки параметром, т.к. работа с моделью не происходит вне класса FacilityService.
        public Factory? FindFactory(string unitName)
        {
            return units?.FirstOrDefault(unit => unit.Name == unitName)?.FactoryId is var factoryId
                        ? factories?.FirstOrDefault(factory => factory.Id == factoryId)
                        : null;
        }
        // Оставляю реализацию т.к. указана в задании.
        public Factory? FindFactory(Unit unit) =>
            factories?.FirstOrDefault(f => f.Id == unit.Id);

        public IFacility? Search(string searchName)
        {
            if (container is null)
                if (factories is null || units is null || tanks is null) return null;
                else container = new(factories, units, tanks);

            var factoryResult = container.Factories
                .FirstOrDefault(Factory => Factory.Name == searchName);

            if (factoryResult != default)
                return factoryResult;

            var unitResult = container.Units
                .FirstOrDefault(Unit => Unit.Name == searchName);

            if (unitResult != default)
                return unitResult;

            var tankResult = container.Tanks.
                FirstOrDefault(Tank => Tank.Name == searchName);
            return tankResult != default ? tankResult : null;
        }
        #endregion
    }
}