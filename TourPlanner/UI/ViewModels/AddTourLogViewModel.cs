using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Utilities;

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
            try
            {
                if (logs == null)
                {
                    MessageBox.Show("No logs found for the selected tour!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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


                MessageBox.Show("Tour log added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error adding tour log: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $"\n\nInner Exception: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            if (Application.Current != null && Application.Current.Windows.Count > 1)
            {
                Application.Current.Windows[1]?.Close();
            }
        }
    }
}
