using Factories.WebApi.BLL.Services;
using Factories.WebApi.DAL.Entities;
using Factories.WebApi.DAL.Interfaces;
using Moq;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Factories.WebApi.Tests
{
    [TestFixture]
    public class WorkerServiceTests
    {
        [Test]
        public async Task UpdateAllVolumesRandomlyAsync_UpdatesVolumesCorrectly()
        {
            // Arrange
            var tanks = new List<Tank>
            {
                new Tank {Name = "Tank1", Volume = 150, MaxVolume = 200 },
                new Tank {Name = "Tank2", Volume = 150, MaxVolume = 300 }
            };

            var tankRepositoryMock = new Mock<IRepository<Tank>>();
            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tanks);

            var workerService = new WorkerService(null);

            // Act
            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            // Assert
            foreach (var tank in tanks)
            {
                Assert.IsTrue(tank.Volume >= 0 && tank.Volume <= tank.MaxVolume, $"Tank volume {tank.Name} is not within valid range.");
            }
        }
    }

    [TestFixture]
    public class TankVolumeUpdateTests
    {
        [Test]
        public void TankVolume_RemainsUnchanged_WhenRandomChangeIsZero()
        {
            // Arrange
            var tank = new Tank {Name="1", Volume = 100, MaxVolume = 200, Unit=new Unit()};
            var randomChange = 0;
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.AreEqual(originalVolume, tank.Volume);
        }

        [TestCase(0.1)]
        [TestCase(0.5)]
        [TestCase(0.9)]
        public void TankVolume_Increases_WhenRandomChangeIsPositive(double randomChange)
        {
            // Arrange
            var tank = new Tank { Id = 1, Name = "1", Volume = 100, MaxVolume = 200 };
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.Greater(tank.Volume, originalVolume);
        }

        [TestCase(-0.1)]
        [TestCase(-0.5)]
        [TestCase(-0.9)]
        public void TankVolume_Decreases_WhenRandomChangeIsNegative(double randomChange)
        {
            // Arrange
            var tank = new Tank { Id = 1, Name = "1", Volume = 100, MaxVolume = 200 };
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.Less(tank.Volume, originalVolume);
        }

        [TestCase(1)]
        [TestCase(-1)]
        public void TankVolume_ReachesMaxOrMin_WhenRandomChangeIsMaxOrMin(double randomChange)
        {
            // Arrange
            var tank = new Tank { Id = 1, Name = "1", Volume = 100, MaxVolume = 200 };

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.AreEqual(randomChange > 0 ? tank.MaxVolume : 0, tank.Volume);
        }

        [TestCase(0.1)]
        [TestCase(-0.1)]
        public void TankVolume_ChangesBySmallAmount_WhenRandomChangeIsCloseToEdge(double randomChange)
        {
            // Arrange
            var tank = new Tank { Id = 1, Name = "1", Volume = 100, MaxVolume = 200 };
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.AreEqual(originalVolume + randomChange * originalVolume, tank.Volume);
        }

        private void UpdateTankVolume(Tank tank, double randomChange)
        {
            tank.Volume += tank.Volume * randomChange;
            // Ensure volume stays within bounds
            if (tank.Volume > tank.MaxVolume)
                tank.Volume = tank.MaxVolume;
            else if (tank.Volume < 0)
                tank.Volume = 0;
        }
    }
}