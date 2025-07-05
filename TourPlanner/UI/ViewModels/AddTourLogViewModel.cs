using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.Utilities;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.ViewModels
{
    public class AddTourLogViewModel
    {
        public DateTime LogDate { get; set; } = DateTime.Now;
        public string LogComment { get; set; } = string.Empty;
        public int LogDifficulty { get; set; } = 1;
        public double LogTotalDistance { get; set; }
        public double LogTotalTime { get; set; }
        public int LogRating { get; set; } = 1;

        private readonly TourLogService logService;
        private readonly ObservableCollection<TourLog> logs;
        private readonly Tour parentTour;
        private readonly IDialogService _dialogService;
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();

        public ICommand ConfirmAddLogCommand { get; }

        public AddTourLogViewModel(TourLogService logService, ObservableCollection<TourLog> logs, Tour parentTour)
        {
            this.logService = logService;
            this.logs = logs;
            this.parentTour = parentTour;
            ConfirmAddLogCommand = new RelayCommand(async _ => await AddLogAsync(), _ => true);
        }

        private async Task AddLogAsync()
        {
              if (logs == null)
              {
                  _dialogService.ShowMessage("No logs found for the selected tour!", "Error");
                  return;
              }
              try
              {
                var newLog = new TourLog
                {
                    Date = LogDate.ToUniversalTime(),
                    Comment = LogComment,
                    Difficulty = LogDifficulty,
                    TotalDistance = LogTotalDistance,
                    TotalTime = LogTotalTime,
                    Rating = LogRating
                };
                parentTour.TourLogs.Add(newLog); // in die Tour einfügen

                // In der DB speichern
                await logService.AddTourLogAsync(newLog);
                _logger.Info($"Tour log added successfully: {LogComment} on {LogDate}");
                _dialogService.ShowMessage("Tour log added successfully!", "Success");
                return;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error adding tour log: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                _logger.Error(errorMsg);
            }
            _dialogService.CloseCurrentWindow();
        }
    }
}
