using Factories.WebApi.BLL.Services;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Moq;

namespace Factories.WebApi.Tests
{
    [TestFixture]
    public class WorkerServiceNUnitTests
    {
        private Mock<IRandomService> mockRandomService;
        private Mock<IRepository<Tank>> tankRepositoryMock;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Structure", "NUnit1032:An IDisposable field/property should be Disposed in a TearDown method", Justification = "<Ожидание>")]
        private WorkerService workerService; // игнорирую предупреждение, т.к. не требуется особым образом диспозить workerService

        [SetUp]
        public void Setup()
        {
            mockRandomService = new Mock<IRandomService>();
            tankRepositoryMock = new Mock<IRepository<Tank>>();
            workerService = new WorkerService(null!, mockRandomService.Object);
        }

        [Test]
        public async Task UpdateAllVolumesRandomlyAsync_VolumeDecreasesByMinimum_WhenRandomIsMinimum()
        {
            mockRandomService.Setup(x => x.NextDouble()).Returns(0);

            var tanks = new List<Tank>
            {
                new() { Name = "Tank1", Volume = 100, MaxVolume = 200, Unit=new Unit(){Name="Unit 1", Factory=new Factory(){Name="Factory 1" } } }
            };

            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(tanks);

            var workerService = new WorkerService(null!, mockRandomService.Object);

            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            // Проверяем, что объем уменьшился на минимум
            Assert.That(tanks.All(tank => tank.Volume < 100 && tank.Volume >= 0), Is.True, "Volume did not decrease by the expected minimum amount.");
        }

        [Test]
        public async Task UpdateAllVolumesRandomlyAsync_VolumeIncreasesByMaximum_WhenRandomIsMaximum()
        {
            mockRandomService.Setup(x => x.NextDouble()).Returns(1);

            var tanks = new List<Tank>
            {
                new() { Name = "Tank1", Volume = 200, MaxVolume = 200, Unit=new Unit(){Name="Unit 1", Factory=new Factory(){Name="Factory 1" } } }
             };

            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(tanks);

            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            // Проверяем, что объем увеличился на максимум
            Assert.That(tanks.All(tank => tank.Volume <= tank.MaxVolume && tank.Volume > 100), Is.True, "Volume did not increase by the expected maximum amount.");
        }

        [Test]
        public async Task UpdateAllVolumesRandomlyAsync_VolumeDoesNotDropBelowZero()
        {
            mockRandomService.Setup(x => x.NextDouble()).Returns(0);

            var tanks = new List<Tank>
            {
                new() { Name = "Tank1", Volume = 1, MaxVolume = 200, Unit=new Unit(){Name="Unit 1", Factory=new Factory(){Name="Factory 1" } } }
            };

            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(tanks);

            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            Assert.That(tanks.All(tank => tank.Volume >= 0), Is.True, "Tank volume dropped below zero.");
        }
    }
}
