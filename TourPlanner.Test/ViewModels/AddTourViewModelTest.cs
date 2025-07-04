using System.Collections.ObjectModel;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.Test.ViewModels
{
    public class AddTourViewModelTest
    {
        private TourService _tourService;
        private ObservableCollection<Tour> _tours;
        [SetUp]
        public void Setup()
        {
            _tourService = new TourService();
            _tours = new ObservableCollection<Tour>();
            DialogService.SuppressDialogs = true;
        }

        [Test]
        public void AddTour_ValidInput_TourIsAddedToCollection()
        {
            // Arrange
            int prevTourCount = _tours.Count();
            var viewModel = new AddTourViewModel(_tourService, _tours)
            {
                NewTourName = "Test Tour",
                NewTourDesc = "A beautiful scenic tour",
                NewTourFrom = "Vienna",
                NewTourTo = "Salzburg",
                NewTourTransportType = TransportType.Train
            };

            // Act
            viewModel.ConfirmAddTourCommand.Execute(null);
            int newTourCount = prevTourCount + 1;

            // Assert
            Assert.That(_tours.Count, Is.EqualTo(newTourCount));
            Assert.That(_tours[0].Name, Is.EqualTo("Test Tour"));
            Assert.That(_tours[0].Description, Is.EqualTo("A beautiful scenic tour"));
            Assert.That(_tours[0].From, Is.EqualTo("Vienna"));
            Assert.That(_tours[0].To, Is.EqualTo("Salzburg"));
            Assert.That(_tours[0].TransportType, Is.EqualTo(TransportType.Train));
        }

        [Test]
        public void AddTour_MissingFields_TourIsNotAdded()
        {
            // Arrange
            int prevTourCount = _tours.Count();
            var viewModel = new AddTourViewModel(_tourService, _tours)
            {
                NewTourName = "",
                NewTourDesc = "",
                NewTourFrom = "",
                NewTourTo = ""
            };

            // Act
            viewModel.ConfirmAddTourCommand.Execute(null);

            // Assert
            Assert.That(_tours.Count, Is.EqualTo(prevTourCount));
        }
    }
}