using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Services;
using Moq;
using TourPlanner.DAL;
using TourPlanner.UI.Services;

namespace TourPlanner.Test.Services
{
    public class TourServiceTest
    {
        private Mock<IRepository<Tour>> _repositoryMock;
        private Mock<IOpenRouteServiceClient> _orsClientMock;
        private TourService _tourService;


        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IRepository<Tour>>();
            _orsClientMock = new Mock<IOpenRouteServiceClient>();
            _tourService = new TourService(_repositoryMock.Object, _orsClientMock.Object);
            DialogService.SuppressDialogs = true;
        }

        [TearDown]
        public void TearDown()
        {
            DialogService.SuppressDialogs = false;
        }
        [Test]
        public async Task GetToursAsync_Initially_ReturnsEmptyList()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Tour>());

            // Act
            var result = await _tourService.GetToursAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task AddTourAsync_ValidTour_TourIsAdded()
        {
            // Arrange
            var tour = new Tour { Name = "Test Tour", From = "Vienna", To = "Graz" };

            _orsClientMock
                .Setup(c => c.GetRouteWithCoordinatesAsync(tour.From, tour.To))
                .ReturnsAsync((
                    100.0,                  
                    2.0,                    
                    "{}",                   
                    (48.210033, 16.363449), 
                    (47.070714, 15.439504)  

                ));

            _repositoryMock
                .Setup(r => r.AddAsync(tour))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourService.AddTourAsync(tour);

            // Assert
            _orsClientMock.Verify(c => c.GetRouteWithCoordinatesAsync(tour.From, tour.To), Times.Once);
            _repositoryMock.Verify(r => r.AddAsync(tour), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            Assert.That(tour.Distance, Is.EqualTo(100.0));
            Assert.That(tour.EstimatedTime, Is.EqualTo(2.0));
        }

        [Test]
        public void AddTourAsync_NullTour_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _tourService.AddTourAsync(null);
            });
        }

        [Test]
        public async Task UpdateTourAsync_ExistingTour_UpdatesSuccessfully()
        {
            // Arrange
            var tour = new Tour { Name = "Test Tour", From = "Vienna", To = "Linz" };

            _orsClientMock
                .Setup(c => c.GetRouteWithCoordinatesAsync(tour.From, tour.To))
                .ReturnsAsync((
                    150.0,                  // Distance
                    2.5,                    // Duration (as double!)
                    "{}",                   // GeoJson
                    (48.210033, 16.363449), // FromCoords
                    (48.30694, 14.28583)    // ToCoords
                ));

            _repositoryMock
                .Setup(r => r.Update(tour));

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourService.UpdateTourAsync(tour);

            // Assert
            _orsClientMock.Verify(c => c.GetRouteWithCoordinatesAsync(tour.From, tour.To), Times.Once);
            _repositoryMock.Verify(r => r.Update(tour), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            Assert.That(tour.Distance, Is.EqualTo(150.0));
            Assert.That(tour.EstimatedTime, Is.EqualTo(2.5));
        }

        [Test]
        public void UpdateTourAsync_NullTour_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _tourService.UpdateTourAsync(null);
            });
        }

        [Test]
        public async Task DeleteTourAsync_ExistingTour_DeletesSuccessfully()
        {
            // Arrange
            var tour = new Tour { Name = "Test Tour" };

            _repositoryMock
                .Setup(r => r.Delete(tour));

            _repositoryMock
                .Setup(r => r.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _tourService.DeleteTourAsync(tour);

            // Assert
            _repositoryMock.Verify(r => r.Delete(tour), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
        }
    }
}
