using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Services;

namespace TourPlanner.Test.Services
{
    public class TourServiceTest
    {
        private TourService _tourService;

        [SetUp]
        public void Setup()
        {
            _tourService = new TourService();
        }

        [Test]
        public void GetTours_Initially_ReturnsEmptyList()
        {
            // Act
            var result = _tourService.GetTours();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void AddTour_ValidTour_TourIsAdded()
        {
            // Arrange
            var tour = new Tour { Name = "Test Tour" };

            // Act
            _tourService.AddTour(tour);

            // Assert
            Assert.That(_tourService.GetTours().Count(), Is.EqualTo(1));
            Assert.That(_tourService.GetTours().First().Name, Is.EqualTo("Test Tour"));
        }

        [Test]
        public void UpdateTour_ExistingTour_UpdatesSuccessfully()
        {
            // Arrange
            var tour = new Tour { Name = "Test Tour", Description = "Old Desc" };
            _tourService.AddTour(tour);

            var updatedTour = new Tour { Name = "Test Tour", Description = "New Desc" };

            // Act
            _tourService.UpdateTour(updatedTour);

            // Assert
            Assert.That(_tourService.GetTours().First().Description, Is.EqualTo("New Desc"));
        }

        [Test]
        public void DeleteTour_ExistingTour_RemovesTour()
        {
            // Arrange
            var tour = new Tour { Name = "Test Tour" };
            _tourService.AddTour(tour);

            // Act
            _tourService.DeleteTour(tour);

            // Assert
            Assert.That(_tourService.GetTours().Count(), Is.EqualTo(0));
        }

        [Test]
        public void DeleteTour_NonExistingTour_DoesNothing()
        {
            // Arrange
            var tour = new Tour { Name = "Nonexistent Tour" };

            // Act
            _tourService.DeleteTour(tour);

            // Assert
            Assert.That(_tourService.GetTours().Count(), Is.EqualTo(0));
        }
    }
}
