using System;
using System.Collections.ObjectModel;
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
        private readonly IDialogService _dialogService;
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();

        public ICommand ConfirmAddLogCommand { get; }

        public AddTourLogViewModel(TourLogService logService, ObservableCollection<TourLog> logs)
        {
            this.logService = logService;
            this.logs = logs;
            _dialogService = new DialogService();
            ConfirmAddLogCommand = new RelayCommand(_ => AddLog(), _ => true);
        }

        private void AddLog()
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
                    Date = LogDate,
                    Comment = LogComment,
                    Difficulty = LogDifficulty,
                    TotalDistance = LogTotalDistance,
                    TotalTime = LogTotalTime,
                    Rating = LogRating
                };
                logs.Add(newLog);

                _logger.Info($"Tour log added successfully: {LogComment} on {LogDate}");
                _dialogService.ShowMessage("Tour log added successfully!", "Success");

            }catch(Exception ex)
            {
                _logger.Error($"Error while adding tour log: ${ex}.");
            }
            _dialogService.CloseCurrentWindow();
        }
    }
}
