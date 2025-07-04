using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Services;

namespace TourPlanner.Test.Services
{
    public class TourLogServiceTests
    {
        private TourLogService _tourLogService;

        [SetUp]
        public void Setup()
        {
            _tourLogService = new TourLogService();
        }

        [Test]
        public void GetTourLogs_Initially_ReturnsEmptyList()
        {
            // Act
            var result = _tourLogService.GetTourLogs();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void AddTourLog_ValidLog_LogIsAdded()
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

            // Act
            _tourLogService.AddTourLog(log);

            // Assert
            Assert.That(_tourLogService.GetTourLogs().Count(), Is.EqualTo(1));
            Assert.That(_tourLogService.GetTourLogs().First().Comment, Is.EqualTo("Test Log"));
        }

        [Test]
        public void UpdateTourLog_ExistingLog_UpdatesSuccessfully()
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
            _tourLogService.AddTourLog(log);

            var updatedLog = new TourLog
            {
                Date = DateTime.Today,
                Comment = "Updated Comment",
                Difficulty = 3,
                TotalDistance = 8,
                TotalTime = 1.5,
                Rating = 5
            };

            // Act
            _tourLogService.UpdateTourLog(updatedLog);

            // Assert
            var firstLog = _tourLogService.GetTourLogs().First();
            Assert.That(firstLog.Comment, Is.EqualTo("Updated Comment"));
            Assert.That(firstLog.Difficulty, Is.EqualTo(3));
            Assert.That(firstLog.TotalDistance, Is.EqualTo(8));
            Assert.That(firstLog.TotalTime, Is.EqualTo(1.5));
            Assert.That(firstLog.Rating, Is.EqualTo(5));
        }

        [Test]
        public void DeleteTourLog_ExistingLog_RemovesLog()
        {
            // Arrange
            var log = new TourLog { Date = DateTime.Now };
            _tourLogService.AddTourLog(log);

            // Act
            _tourLogService.DeleteTourLog(log);

            // Assert
            Assert.That(_tourLogService.GetTourLogs().Count(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteTourLog_NonExistingLog_DoesNothing()
        {
            // Arrange
            var log = new TourLog { Date = DateTime.Now };

            // Act
            _tourLogService.DeleteTourLog(log);

            // Assert
            Assert.That(_tourLogService.GetTourLogs().Count(), Is.EqualTo(0));
        }
    }

}
