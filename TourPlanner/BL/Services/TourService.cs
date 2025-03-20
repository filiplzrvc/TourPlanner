using System;
using System.Collections.Generic;

namespace TourPlanner.BL.Services
{
    public class TourService
    {
        private readonly List<Tour> tours = new();
        public IEnumerable<Tour> GetTours() => tours;
        public void AddTour(Tour tour) => tours.Add(tour);
        
        public void UpdateTour(Tour updatedTour)
        {
            var existingTour = tours.FirstOrDefault(t => t.Name == updatedTour.Name);
            if (existingTour != null)
            {
                existingTour.Name = updatedTour.Name;
                existingTour.Description = updatedTour.Description;
                existingTour.From = updatedTour.From;
                existingTour.To = updatedTour.To;
                existingTour.TransportType = updatedTour.TransportType;
                existingTour.Distance = updatedTour.Distance;
                existingTour.EstimatedTime = updatedTour.EstimatedTime;
            }
        }


        public void DeleteTour(Tour tour) => tours.Remove(tour);
    }
}
