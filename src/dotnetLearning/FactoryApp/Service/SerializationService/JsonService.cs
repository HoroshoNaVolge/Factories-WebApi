using dotnetLearning.FactoryApp.Model;
using dotnetLearning.FactoryApp.Service.FacilityService;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace dotnetLearning.FactoryApp.Service.SerializationService
{
    public class JsonService(IOptions<FacilityServiceOptions> options) : ISerializationService
    {
        private readonly string jsonFilePath = options.Value.FacilitiesJsonFilePath!;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true,
        };

        public async Task CreateOrUpdateAllAsync(FacilitiesContainer container, CancellationToken token)
        {
            if (container is null) return;

            string json = JsonSerializer.Serialize(container, jsonSerializerOptions);

            using var writer = new StreamWriter(jsonFilePath);

            await writer.WriteAsync(json);
        }

        public async Task GetFacilitiesAsync(FacilitiesContainer container, CancellationToken token)
        {
            FacilitiesContainer deselializedContainer = container;
            if (!File.Exists(jsonFilePath)) return;

            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                deselializedContainer = await JsonSerializer.DeserializeAsync<FacilitiesContainer>(stream, cancellationToken: token) ?? throw new ArgumentException(MessageConstants.DeserializationErrorMessage);
            }
            container.Factories = deselializedContainer.Factories;
            container.Units = deselializedContainer.Units;
            container.Tanks = deselializedContainer.Tanks;
        }

        public async Task UpdateFacilityAsync(IFacility facility, CancellationToken token)
        {
            if (facility is null || !File.Exists(jsonFilePath)) return;

            FacilitiesContainer? containerToUpdate = null;

            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                containerToUpdate = await JsonSerializer.DeserializeAsync<FacilitiesContainer>(stream, cancellationToken: token) ?? throw new ArgumentException(MessageConstants.DeserializationErrorMessage);

                switch (facility)
                {
                    case Factory factory:
                        var factoryIndex = containerToUpdate.Factories.ToList().FindIndex(f => f.Id == facility.Id);
                        if (factoryIndex != -1)
                        {
                            // Потому что свойства Id и Name объекта в модели неизменяемы
                            var updatedFactory = new Factory
                            {
                                Id = facility.Id,
                                Name = factory.Name,
                                Description = factory.Description
                            };
                            containerToUpdate.Factories[factoryIndex] = updatedFactory;
                        };
                        break;

                    case Unit unit:
                        var unitIndex = containerToUpdate.Units.ToList().FindIndex(u => u.Id == facility.Id);
                        if (unitIndex != -1)
                        {
                            // Потому что свойства Id и Name объекта в модели неизменяемы
                            var updatedUnit = new Unit
                            {
                                Id = facility.Id,
                                Name = unit.Name,
                                Description = unit.Description,
                                FactoryId = unit.FactoryId
                            };
                            containerToUpdate.Units[unitIndex] = updatedUnit;
                        };
                        break;

                    case Tank tank:
                        var tankIndex = containerToUpdate.Tanks.ToList().FindIndex(t => t.Id == facility.Id);
                        if (tankIndex != -1)
                        {
                            // Потому что свойства Id и Name объекта в модели неизменяемы
                            var updatedTank = new Tank
                            {
                                Id = facility.Id,
                                Name = tank.Name,
                                Description = tank.Description,
                                Volume = tank.Volume,
                                MaxVolume = tank.MaxVolume,
                                UnitId = tank.UnitId
                            };
                            containerToUpdate.Tanks[tankIndex] = updatedTank;
                        };
                        break;
                    default:
                        throw new ArgumentException(MessageConstants.InvalidFacilityTypeErrorMessage);
                }
            }
            await CreateOrUpdateAllAsync(containerToUpdate, token);
        }

        public async Task AddFacilityAsync(IFacility facility, CancellationToken token)
        {
            if (facility is null || !File.Exists(jsonFilePath)) return;

            FacilitiesContainer? containerToUpdate = null;

            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                containerToUpdate = await JsonSerializer.DeserializeAsync<FacilitiesContainer>(stream, cancellationToken: token) ?? throw new ArgumentException(MessageConstants.DeserializationErrorMessage);

                switch (facility)
                {
                    case Factory factory:
                        containerToUpdate.Factories.Add(factory);
                        break;
                    case Unit unit:
                        containerToUpdate.Units.Add(unit);
                        break;
                    case Tank tank:
                        containerToUpdate.Tanks.Add(tank);
                        break;
                    default:
                        throw new ArgumentException(MessageConstants.InvalidFacilityTypeErrorMessage);
                }
            }
            await CreateOrUpdateAllAsync(containerToUpdate, token);
        }

        public async Task DeleteFacilityAsync(IFacility facility, CancellationToken token)
        {
            if (facility is null || !File.Exists(jsonFilePath)) return;

            FacilitiesContainer? containerToUpdate = null;

            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                containerToUpdate = await JsonSerializer.DeserializeAsync<FacilitiesContainer>(stream, cancellationToken: token) ?? throw new ArgumentException(MessageConstants.DeserializationErrorMessage);

                switch (facility)
                {
                    case Factory factory:
                        var factoriesToRemove = containerToUpdate.Factories.Where(f => f.Name == factory.Name).ToList();
                        foreach (var factoryToRemove in factoriesToRemove)
                            containerToUpdate.Factories.Remove(factoryToRemove);
                        break;
                    case Unit unit:
                        var unitsToRemove = containerToUpdate.Units.Where(u => u.Name == unit.Name).ToList();
                        foreach (var unitToRemove in unitsToRemove)
                            containerToUpdate.Units.Remove(unitToRemove);
                        break;
                    case Tank tank:
                        var tanksToRemove = containerToUpdate.Tanks.Where(t => t.Name == tank.Name).ToList();
                        foreach (var tankToRemove in tanksToRemove)
                            containerToUpdate.Tanks.Remove(tankToRemove);
                        break;
                    default:
                        throw new ArgumentException(MessageConstants.InvalidFacilityTypeErrorMessage);
                }
            }
            await CreateOrUpdateAllAsync(containerToUpdate, token);
        }
    }
}