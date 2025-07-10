using Moq;
using System.Collections.ObjectModel;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.UI.Services;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.Test.ViewModels
{
    public class AddTourViewModelTest
    {
        private TourService _tourService;
        private ObservableCollection<Tour> _tours;

        private Mock<IRepository<Tour>> _repositoryMock;
        private Mock<IOpenRouteServiceClient> _orsClientMock;
        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Tour>>();
            _orsClientMock = new Mock<IOpenRouteServiceClient>();
            _tourService = new TourService(_repositoryMock.Object, _orsClientMock.Object);
            _tours = new ObservableCollection<Tour>();
            DialogService.SuppressDialogs = true;
        }

        [TearDown]
        public void TearDown()
        {
            DialogService.SuppressDialogs = false;
        }


        [Test]
        public void AddTour_ValidInput_TourIsAddedToCollection()
        {
            // Arrange
            int prevTourCount = _tours.Count;

            _orsClientMock
                .Setup(c => c.GetRouteWithCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((
                    300.0,           
                    5.0,             
                    "{}",            
                    (48.2, 16.3),    
                    (47.8, 13.0)     
                ));

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

    [Test]
        public void AddTour_ValidInput_TourIsAddedToCollection2()
        {
            // Arrange
            int prevTourCount = _tours.Count;

            _orsClientMock
                .Setup(c => c.GetRouteWithCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((
                    250.0,             
                    3.5,               
                    "{}",              
                    (48.2082, 16.3738),
                    (47.8095, 13.0550) 
                ));

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Tour>()))
                .Returns(Task.CompletedTask);
            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            var viewModel = new AddTourViewModel(_tourService, _tours)
            {
                NewTourName = "Scenic Train Ride",
                NewTourDesc = "A picturesque route through the Alps.",
                NewTourFrom = "Vienna",
                NewTourTo = "Salzburg",
                NewTourTransportType = TransportType.Train
            };

            // Act
            viewModel.ConfirmAddTourCommand.Execute(null);

            int newTourCount = prevTourCount + 1;

            // Assert
            Assert.That(_tours.Count, Is.EqualTo(newTourCount), "Tour count should have increased by 1.");
            var addedTour = _tours[0];
            Assert.That(addedTour.Name, Is.EqualTo("Scenic Train Ride"));
            Assert.That(addedTour.Description, Is.EqualTo("A picturesque route through the Alps."));
            Assert.That(addedTour.From, Is.EqualTo("Vienna"));
            Assert.That(addedTour.To, Is.EqualTo("Salzburg"));
            Assert.That(addedTour.TransportType, Is.EqualTo(TransportType.Train));
            Assert.That(addedTour.Distance, Is.EqualTo(250.0));
            Assert.That(addedTour.EstimatedTime, Is.EqualTo(3.5));
        }
    }
}
