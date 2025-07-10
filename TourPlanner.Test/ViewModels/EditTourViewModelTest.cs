using Moq;
using System.Collections.ObjectModel;
using TourPlanner.BL.Services;
using TourPlanner.DAL;
using TourPlanner.UI.Services;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.Test.ViewModels
{
    public class EditTourViewModelTest
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
        public void UpdateTour_ValidInput_TourIsUpdated()
        {
            // Arrange
            var originalTour = new Tour
            {
                Name = "Old Name",
                Description = "Old Description",
                From = "Old From",
                To = "Old To",
                TransportType = TransportType.Bus,
                Distance = 100,
                EstimatedTime = 120
            };

            _orsClientMock
                .Setup(c => c.GetRouteWithCoordinatesAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((
                    200.0,            
                    4.0,              
                    "{}",             
                    (48.2, 16.3),     
                    (47.1, 15.4)      
                ));

            var viewModel = new EditTourViewModel(_tourService, originalTour)
            {
                EditedTour = {
            Name = "New Name",
            Description = "New Description",
            From = "New From",
            To = "New To",
            TransportType = TransportType.Train,
            Distance = 200,
            EstimatedTime = 240
        }
            };

            // Act
            viewModel.ConfirmEditTourCommand.Execute(null);

            // Assert
            Assert.That(originalTour.Name, Is.EqualTo("New Name"));
            Assert.That(originalTour.Description, Is.EqualTo("New Description"));
            Assert.That(originalTour.From, Is.EqualTo("New From"));
            Assert.That(originalTour.To, Is.EqualTo("New To"));
            Assert.That(originalTour.TransportType, Is.EqualTo(TransportType.Train));
            Assert.That(originalTour.Distance, Is.EqualTo(200));    
            Assert.That(originalTour.EstimatedTime, Is.EqualTo(4.0)); 
        }


        [Test]
        public void UpdateTour_MissingFields_TourIsNotUpdated()
        {
            // Arrange
            var originalTour = new Tour
            {
                Name = "Valid Name",
                Description = "Valid Description",
                From = "Valid From",
                To = "Valid To"
            };
            var viewModel = new EditTourViewModel(_tourService, originalTour)
            {
                EditedTour = {
                    Name = "",
                    Description = "",
                    From = "",
                    To = ""
                }
            };

            // Act
            viewModel.ConfirmEditTourCommand.Execute(null);

            // Assert
            Assert.That(originalTour.Name, Is.EqualTo("Valid Name"));
            Assert.That(originalTour.Description, Is.EqualTo("Valid Description"));
            Assert.That(originalTour.From, Is.EqualTo("Valid From"));
            Assert.That(originalTour.To, Is.EqualTo("Valid To"));
        }
    }
}

