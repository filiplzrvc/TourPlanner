using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.DAL;

namespace TourPlanner.BL.Services
{
    public class TourService
    {
        private readonly IRepository<Tour> repository;
        private readonly OpenRouteServiceClient orsClient;

        public TourService(IRepository<Tour> repository, OpenRouteServiceClient orsClient)
        {
            this.repository = repository;
            this.orsClient = orsClient;
        }

        public async Task<IEnumerable<Tour>> GetToursAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task AddTourAsync(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            try
            {
                var (distance, duration, _, _, _) = await orsClient.GetRouteWithCoordinatesAsync(tour.From, tour.To);
                tour.Distance = distance;
                tour.EstimatedTime = duration;
            }
            catch (Exception ex)
            {
                tour.Distance = 0;
                tour.EstimatedTime = 0;
                throw new Exception($"Route konnte nicht berechnet werden: {ex.Message}");
            }

            await repository.AddAsync(tour);
            await repository.SaveAsync();
        }

        public async Task UpdateTourAsync(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            try
            {
                var (distance, duration, _, _, _) = await orsClient.GetRouteWithCoordinatesAsync(tour.From, tour.To);
                tour.Distance = distance;
                tour.EstimatedTime = duration;
            }
            catch (Exception ex)
            {
                tour.Distance = 0;
                tour.EstimatedTime = 0;
                throw new Exception($"Route konnte nicht berechnet werden: {ex.Message}");
            }

            repository.Update(tour);
            await repository.SaveAsync();
        }


        public async Task DeleteTourAsync(Tour tour)
        {
            repository.Delete(tour);
            await repository.SaveAsync();
        }
    }
}
