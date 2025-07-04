using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.Test.ViewModels
{
    public class AddTourLogViewModelTests
    {
        private TourLogService _logService;
        private ObservableCollection<TourLog> _logs;

        [SetUp]
        public void Setup()
        {
            _logService = new TourLogService();
            _logs = new ObservableCollection<TourLog>();
            DialogService.SuppressDialogs = true; 
        }

        [TearDown]
        public void TearDown()
        {
            DialogService.SuppressDialogs = false; 
        }

        [Test]
        public void AddLog_ValidLog_LogIsAdded()
        {
            // Arrange
            var vm = new AddTourLogViewModel(_logService, _logs)
            {
                LogComment = "Test Comment",
                LogDifficulty = 2,
                LogTotalDistance = 10,
                LogTotalTime = 1.5,
                LogRating = 4
            };

            int initialCount = _logs.Count;

            // Act
            vm.ConfirmAddLogCommand.Execute(null);

            // Assert
            Assert.That(_logs.Count, Is.EqualTo(initialCount + 1));
            Assert.That(_logs[0].Comment, Is.EqualTo("Test Comment"));
        }

        [Test]
        public void AddLog_NullLogs_ShowsErrorMessageAndDoesNotThrow()
        {
            // Arrange
            var vm = new AddTourLogViewModel(_logService, null);

            // Act + Assert
            Assert.DoesNotThrow(() => vm.ConfirmAddLogCommand.Execute(null));
        }

        [Test]
        public void AddLog_AddedLogHasCorrectData()
        {
            // Arrange
            var vm = new AddTourLogViewModel(_logService, _logs)
            {
                LogDate = new DateTime(2024, 1, 1),
                LogComment = "Verify Data",
                LogDifficulty = 3,
                LogTotalDistance = 12.5,
                LogTotalTime = 2,
                LogRating = 5
            };

            // Act
            vm.ConfirmAddLogCommand.Execute(null);

            // Assert
            var addedLog = _logs.FirstOrDefault();
            Assert.That(addedLog, Is.Not.Null);
            Assert.That(addedLog.Date, Is.EqualTo(new DateTime(2024, 1, 1)));
            Assert.That(addedLog.Comment, Is.EqualTo("Verify Data"));
            Assert.That(addedLog.Difficulty, Is.EqualTo(3));
            Assert.That(addedLog.TotalDistance, Is.EqualTo(12.5));
            Assert.That(addedLog.TotalTime, Is.EqualTo(2));
            Assert.That(addedLog.Rating, Is.EqualTo(5));
        }

    }
}
