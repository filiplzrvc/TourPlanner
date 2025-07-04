using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.Utilities;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.ViewModels
{
    public class EditTourLogViewModel
    {
        private readonly TourLogService logService;
        private readonly TourLog originalLog;
        private readonly IDialogService _dialogService;
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();

        public TourLog EditedLog { get; }

        public ICommand ConfirmEditLogCommand { get; }

        public EditTourLogViewModel(TourLogService logService, TourLog selectedLog)
        {
            this.logService = logService;
            originalLog = selectedLog;

            EditedLog = new TourLog
            {
                Date = selectedLog.Date,
                Comment = selectedLog.Comment,
                Difficulty = selectedLog.Difficulty,
                TotalDistance = selectedLog.TotalDistance,
                TotalTime = selectedLog.TotalTime,
                Rating = selectedLog.Rating
            };
            _dialogService = new DialogService();

            ConfirmEditLogCommand = new RelayCommand(_ => UpdateLog());
        }

        private void UpdateLog()
        {
            _logger.Info($"EditTourLog started for date {originalLog.Date}");
            try
            {
                originalLog.Date = EditedLog.Date;
                originalLog.Comment = EditedLog.Comment;
                originalLog.Difficulty = EditedLog.Difficulty;
                originalLog.TotalDistance = EditedLog.TotalDistance;
                originalLog.TotalTime = EditedLog.TotalTime;
                originalLog.Rating = EditedLog.Rating;

                logService.UpdateTourLog(EditedLog);
                _dialogService.ShowMessage($"Tour log (Date: [{originalLog.Date}]) updated successfully!", "Success");
            }catch(Exception ex)
            {
                _logger.Error($"Error while updating tour log: {ex}");
            }

        }
    }
}
