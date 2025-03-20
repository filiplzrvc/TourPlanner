using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TourPlanner.BL.Services
{
    public class TourLogService
    {
        private readonly List<TourLog> tourLogs = new();

        public IEnumerable<TourLog> GetTourLogs() => tourLogs;

        public void AddTourLog(TourLog log) => tourLogs.Add(log);

        public void UpdateTourLog(TourLog updatedLog)
        {
            var existingLog = tourLogs.Find(l => l.Date == updatedLog.Date);
            if (existingLog != null)
            {
                existingLog.Comment = updatedLog.Comment;
                existingLog.Difficulty = updatedLog.Difficulty;
                existingLog.TotalDistance = updatedLog.TotalDistance;
                existingLog.TotalTime = updatedLog.TotalTime;
                existingLog.Rating = updatedLog.Rating;
            }
        }

        public void DeleteTourLog(TourLog log) => tourLogs.Remove(log);
    }
}

