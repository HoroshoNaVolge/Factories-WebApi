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
                new() {Name = "Tank1", Volume = 150, MaxVolume = 200, Unit =new Unit(){Name="Test", Factory = new Factory(){ Name="Test"} } },
                new() {Name = "Tank2", Volume = 150, MaxVolume = 300, Unit =new Unit(){Name="Test", Factory = new Factory(){ Name="Test"} } }
            };

            var tankRepositoryMock = new Mock<IRepository<Tank>>();
            tankRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tanks);

            var workerService = new WorkerService(null);

            // Act
            await workerService.UpdateAllVolumesRandomlyAsync(tankRepositoryMock.Object, CancellationToken.None);

            // Assert
            foreach (var tank in tanks)
                Assert.That(tank.Volume >= 0 && tank.Volume <= tank.MaxVolume, Is.True, $"Tank volume {tank.Name} is not within valid range.");

        }
    }

    [TestFixture]
    public class TankVolumeUpdateTests
    {
        [Test]
        public void TankVolume_RemainsUnchanged_WhenRandomChangeIsZero()
        {
            // Arrange
            var tank = new Tank { Name = "1", Volume = 100, MaxVolume = 200, Unit = new Unit() { Name = "Test", Factory = new Factory() { Name = "Test" } } };
            var randomChange = 0;
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.That(tank.Volume, Is.EqualTo(originalVolume));
        }

        [TestCase(0.1)]
        [TestCase(0.5)]
        [TestCase(0.9)]
        public void TankVolume_Increases_WhenRandomChangeIsPositive(double randomChange)
        {
            // Arrange
            var tank = new Tank { Name = "1", Volume = 100, MaxVolume = 200, Unit = new Unit() { Name = "Test", Factory = new Factory() { Name = "Test" } } };
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.That(tank.Volume, Is.GreaterThan(originalVolume));
        }

        [TestCase(-0.1)]
        [TestCase(-0.5)]
        [TestCase(-0.9)]
        public void TankVolume_Decreases_WhenRandomChangeIsNegative(double randomChange)
        {
            // Arrange
            var tank = new Tank { Name = "1", Volume = 100, MaxVolume = 200, Unit = new Unit() { Name = "Test", Factory = new Factory() { Name = "Test" } } };
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.That(tank.Volume, Is.LessThan(originalVolume));
        }

        [TestCase(1)]
        [TestCase(-1)]
        public void TankVolume_ReachesMaxOrMin_WhenRandomChangeIsMaxOrMin(double randomChange)
        {
            // Arrange
            var tank = new Tank { Name = "1", Volume = 100, MaxVolume = 200, Unit = new Unit() { Name = "Test", Factory = new Factory() { Name = "Test" } } };

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.That(tank.Volume, Is.EqualTo(randomChange > 0 ? tank.MaxVolume : 0));
        }

        [TestCase(0.1)]
        [TestCase(-0.1)]
        public void TankVolume_ChangesBySmallAmount_WhenRandomChangeIsCloseToEdge(double randomChange)
        {
            // Arrange
            var tank = new Tank { Name = "1", Volume = 100, MaxVolume = 200, Unit = new Unit() { Name = "Test", Factory = new Factory() { Name = "Test" } } };
            var originalVolume = tank.Volume;

            // Act
            UpdateTankVolume(tank, randomChange);

            // Assert
            Assert.That(tank.Volume, Is.EqualTo(originalVolume + randomChange * originalVolume));
        }

        private static void UpdateTankVolume(Tank tank, double randomChange)
        {
            tank.Volume += tank.Volume * randomChange;

            if (tank.Volume > tank.MaxVolume)
                tank.Volume = tank.MaxVolume;
            else if (tank.Volume < 0)
                tank.Volume = 0;
        }
    }
}
