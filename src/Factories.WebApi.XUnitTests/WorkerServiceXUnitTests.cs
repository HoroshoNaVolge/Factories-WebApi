using Factories.WebApi.BLL.Services;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Moq;

namespace Factories.WebApi.XUnitTests
{
    public class WorkerServiceXUnitTests
    {
        private readonly Mock<IRandomService> mockRandomService;
        private readonly Mock<IRepository<Tank>> tankRepositoryMock;
        private readonly WorkerService workerService;

        public WorkerServiceXUnitTests()
        {
            mockRandomService = new Mock<IRandomService>();
            tankRepositoryMock = new Mock<IRepository<Tank>>();
            workerService = new WorkerService(null!, mockRandomService.Object);
        }

        [Fact]
        public async Task UpdateAllVolumesRandomlyAsync_VolumeDecreasesByMinimum_WhenRandomIsMinimum()
        {
            mockRandomService.Setup(x => x.NextDouble()).Returns(0);

            var tanks = new List<Tank>
            {
                new() { Name = "Tank1", Volume = 100, MaxVolume = 200, Unit=new Unit(){Name="Unit 1", Factory=new Factory(){Name="Factory 1" } } }
            };

            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(tanks);

            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            Assert.All(tanks, tank =>
            {
                var volume = tank.Volume;
                Assert.True(volume >= 0 && volume <= 99, $"Tank {tank.Name} volume {volume} is out of range.");
            });
        }

        [Fact]
        public async Task UpdateAllVolumesRandomlyAsync_VolumeIncreasesByMaximum_WhenRandomIsMaximum()
        {
            mockRandomService.Setup(x => x.NextDouble()).Returns(1);

            var tanks = new List<Tank>
            {
                new() { Name = "Tank1", Volume = 199, MaxVolume = 200, Unit=new Unit(){Name="Unit 1", Factory=new Factory(){Name="Factory 1" } } }
             };

            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(tanks);

            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            Assert.True(tanks.All(tank => tank.Volume <= tank.MaxVolume && tank.Volume > 100), "Volume did not increase by the expected maximum amount.");
        }

        [Fact]
        public async Task UpdateAllVolumesRandomlyAsync_VolumeDoesNotDropBelowZero()
        {
            mockRandomService.Setup(x => x.NextDouble()).Returns(0);

            var tanks = new List<Tank>
            {
                new() { Name = "Tank1", Volume = 1, MaxVolume = 200, Unit=new Unit(){Name="Unit 1", Factory=new Factory(){Name="Factory 1" } } }
            };

            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))!.ReturnsAsync(tanks);

            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            Assert.True(tanks.All(tank => tank.Volume >= 0), "Tank volume dropped below zero.");
        }
    }
}
