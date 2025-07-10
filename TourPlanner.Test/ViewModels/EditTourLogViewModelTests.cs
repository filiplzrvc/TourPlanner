using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.Test.ViewModels
{
    public class EditTourLogViewModelTests
    {
        private TourLogService _logService;
        private TourLog _originalLog;


        [SetUp]
        public void Setup()
        {
            _logService = new TourLogService();
            _originalLog = new TourLog
            {
                Date = System.DateTime.Now,
                Comment = "Old Comment",
                Difficulty = 2,
                TotalDistance = 5,
                TotalTime = 1,
                Rating = 3
            };

            DialogService.SuppressDialogs = true; 
        }

        [TearDown]
        public void TearDown()
        {
            DialogService.SuppressDialogs = false; 
        }

        [Test]
        public void UpdateLog_ValidEdit_UpdatesOriginalLog()
        {
            // Arrange
            var vm = new EditTourLogViewModel(_logService, _originalLog);
            vm.EditedLog.Comment = "Updated Comment";
            vm.EditedLog.Difficulty = 4;

            // Act
            vm.ConfirmEditLogCommand.Execute(null);

            // Assert
            Assert.That(_originalLog.Comment, Is.EqualTo("Updated Comment"));
            Assert.That(_originalLog.Difficulty, Is.EqualTo(4));
        }

        [Test]
        public void ConfirmEditLogCommand_ExecutesWithoutError()
        {
            // Arrange
            var vm = new EditTourLogViewModel(_logService, _originalLog);

            // Act + Assert
            Assert.DoesNotThrow(() => vm.ConfirmEditLogCommand.Execute(null));
        }

        [Test]
        public void UpdateLog_ChangesOriginalLogReference()
        {
            // Arrange
            var vm = new EditTourLogViewModel(_logService, _originalLog);
            vm.EditedLog.Comment = "Changed Reference";

            // Act
            vm.ConfirmEditLogCommand.Execute(null);

            // Assert
            Assert.That(_originalLog.Comment, Is.EqualTo("Changed Reference"));
        }

    }
}
