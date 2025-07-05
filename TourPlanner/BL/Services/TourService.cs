using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.DAL;

namespace TourPlanner.BL.Services
{
    public class TourService
    {
        private readonly IRepository<Tour> repository;

        public TourService(IRepository<Tour> repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<Tour>> GetToursAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task AddTourAsync(Tour tour)
        {
            await repository.AddAsync(tour);
            await repository.SaveAsync();
        }

        public async Task UpdateTourAsync(Tour tour)
        {
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
