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

            ConfirmEditLogCommand = new RelayCommand(_ => UpdateLog());
        }

        private void UpdateLog()
        {
            originalLog.Date = EditedLog.Date;
            originalLog.Comment = EditedLog.Comment;
            originalLog.Difficulty = EditedLog.Difficulty;
            originalLog.TotalDistance = EditedLog.TotalDistance;
            originalLog.TotalTime = EditedLog.TotalTime;
            originalLog.Rating = EditedLog.Rating;

            logService.UpdateTourLog(EditedLog);

            MessageBox.Show("Tour log updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Application.Current.Windows[1]?.Close();
        }
    }
}
