using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.UI.ViewModels;

namespace TourPlanner.Test.ViewModels
{
    public class EditTourViewModelTest
    {
        public void Setup()
        {
            DialogService.SuppressDialogs = true;
        }
        [Test]
        public void UpdateTour_ValidInput_TourIsUpdated()
        {
            // Arrange
            var tourService = new TourService();
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
            var viewModel = new EditTourViewModel(tourService, originalTour)
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
            Assert.That(originalTour.EstimatedTime, Is.EqualTo(240));
        }

        [Test]
        public void UpdateTour_MissingFields_TourIsNotUpdated()
        {
            // Arrange
            var tourService = new TourService();
            var originalTour = new Tour
            {
                Name = "Valid Name",
                Description = "Valid Description",
                From = "Valid From",
                To = "Valid To"
            };
            var viewModel = new EditTourViewModel(tourService, originalTour)
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

