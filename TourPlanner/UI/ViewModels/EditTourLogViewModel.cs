using System;
using System.Threading.Tasks;
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
            this.logService = logService ?? throw new ArgumentNullException(nameof(logService));
            originalLog = selectedLog ?? throw new ArgumentNullException(nameof(selectedLog));

            // Kopie erstellen, damit Änderungen erst nach Bestätigung übernommen werden
            EditedLog = new TourLog
            {
                Id = selectedLog.Id, // wichtig: Id mitgeben, damit EF weiß welches Entity
                Date = selectedLog.Date,
                Comment = selectedLog.Comment,
                Difficulty = selectedLog.Difficulty,
                TotalDistance = selectedLog.TotalDistance,
                TotalTime = selectedLog.TotalTime,
                Rating = selectedLog.Rating
            };
            _dialogService = new DialogService();

            ConfirmEditLogCommand = new RelayCommand(async _ => await UpdateLogAsync());
        }

        private async Task UpdateLogAsync()
        {
           
            try
            {
               // Werte aus der bearbeiteten Kopie zurück in das Original übernehmen
                originalLog.Date = EditedLog.Date;
                originalLog.Comment = EditedLog.Comment;
                originalLog.Difficulty = EditedLog.Difficulty;
                originalLog.TotalDistance = EditedLog.TotalDistance;
                originalLog.TotalTime = EditedLog.TotalTime;
                originalLog.Rating = EditedLog.Rating;
              
                await logService.UpdateTourLogAsync(originalLog);

                _dialogService.ShowMessage($"Tour log (Date: [{originalLog.Date}]) updated successfully!", "Success");
                _logger.Info($"Tour log (Date: [{originalLog.Date}]) updated successfully!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Failed to update tour log: {ex.Message}", "Error");
                _logger.Error($"Error while updating tour log: {ex}");
            }
            _dialogService.CloseCurrentWindow();
          
        }

    }
}
