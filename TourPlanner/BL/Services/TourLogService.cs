using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.DAL;

namespace TourPlanner.BL.Services
{
    public class TourLogService
    {
        private readonly IRepository<TourLog> repository;
        private readonly List<TourLog> _logs;

        public TourLogService(IRepository<TourLog> repository)
        {
            this.repository = repository;
        }

        public TourLogService()
        {
            _logs = new List<TourLog>();
        }

        public async Task<IEnumerable<TourLog>> GetTourLogsAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task AddTourLogAsync(TourLog log)
        {
            await repository.AddAsync(log);
            await repository.SaveAsync();
        }

        public async Task UpdateTourLogAsync(TourLog log)
        {
            repository.Update(log);
            await repository.SaveAsync();
        }

        public async Task DeleteTourLogAsync(TourLog log)
        {
            repository.Delete(log);
            await repository.SaveAsync();
        }
    }
}
