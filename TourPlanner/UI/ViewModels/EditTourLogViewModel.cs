using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Utilities;

namespace TourPlanner.UI.ViewModels
{
    public class EditTourLogViewModel
    {
        private readonly TourLogService logService;
        private readonly TourLog originalLog;
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

            ConfirmEditLogCommand = new RelayCommand(async _ => await UpdateLogAsync());
        }

        private async Task UpdateLogAsync()
        {
            // Werte aus der bearbeiteten Kopie zurück in das Original übernehmen
            originalLog.Date = EditedLog.Date;
            originalLog.Comment = EditedLog.Comment;
            originalLog.Difficulty = EditedLog.Difficulty;
            originalLog.TotalDistance = EditedLog.TotalDistance;
            originalLog.TotalTime = EditedLog.TotalTime;
            originalLog.Rating = EditedLog.Rating;

            try
            {
                await logService.UpdateTourLogAsync(originalLog);

                MessageBox.Show("Tour log updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update tour log: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            if (Application.Current.Windows.Count > 1)
            {
                Application.Current.Windows[1]?.Close();
            }
        }
    }
}
