using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.UI.Services;

namespace TourPlanner.Test.Services
{
    public class TourLogServiceTests
    {
        private Mock<IRepository<TourLog>> _repositoryMock;
        private TourLogService _tourLogService;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<TourLog>>();
            _tourLogService = new TourLogService(_repositoryMock.Object);
            DialogService.SuppressDialogs = true;
        }

        [TearDown]
        public void TearDown()
        {
            DialogService.SuppressDialogs = false;
        }
        [Test]
        public async Task GetTourLogsAsync_Initially_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<TourLog>());

            // Act
            var result = await _tourLogService.GetTourLogsAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task AddTourLogAsync_ValidLog_LogIsAdded()
        {
            // Arrange
            var log = new TourLog
            {
                Date = DateTime.Now,
                Comment = "Test Log",
                Difficulty = 2,
                TotalDistance = 10.5,
                TotalTime = 1.25,
                Rating = 4
            };

            _repositoryMock
                .Setup(r => r.AddAsync(log))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourLogService.AddTourLogAsync(log);

            // Assert
            _repositoryMock.Verify(r => r.AddAsync(log), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateTourLogAsync_ExistingLog_UpdatesSuccessfully()
        {
            // Arrange
            var log = new TourLog
            {
                Date = DateTime.Today,
                Comment = "Old Comment",
                Difficulty = 1,
                TotalDistance = 5,
                TotalTime = 0.75,
                Rating = 3
            };

            _repositoryMock
                .Setup(r => r.Update(log));

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourLogService.UpdateTourLogAsync(log);

            // Assert
            _repositoryMock.Verify(r => r.Update(log), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteTourLogAsync_ExistingLog_RemovesLog()
        {
            // Arrange
            var log = new TourLog { Date = DateTime.Now };

            _repositoryMock
                .Setup(r => r.Delete(log));

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourLogService.DeleteTourLogAsync(log);

            // Assert
            _repositoryMock.Verify(r => r.Delete(log), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteTourLogAsync_NonExistingLog_DoesNothing()
        {
            // Arrange
            var log = new TourLog { Date = DateTime.Now };

            _repositoryMock
                .Setup(r => r.Delete(log));

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourLogService.DeleteTourLogAsync(log);

            // Assert
            _repositoryMock.Verify(r => r.Delete(log), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }

    }

}
