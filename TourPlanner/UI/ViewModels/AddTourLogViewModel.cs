using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
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
        private readonly IDialogService _dialogService;

        public ICommand ConfirmAddLogCommand { get; }

        private bool IsRunningInTest()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName.ToLowerInvariant().Contains("nunit")
                       || a.FullName.ToLowerInvariant().Contains("xunit")
                       || a.FullName.ToLowerInvariant().Contains("mstest"));
        }
        public AddTourLogViewModel(TourLogService logService, ObservableCollection<TourLog> logs)
        {
            this.logService = logService;
            this.logs = logs;
            _dialogService = new DialogService();
            ConfirmAddLogCommand = new RelayCommand(_ => AddLog(), _ => true);
        }

        //private bool CanAddLog()
        //{
        //    return !string.IsNullOrWhiteSpace(LogComment);
        //}

        private void AddLog()
        {
            if (logs == null)
            {
                _dialogService.ShowMessage("No logs found for the selected tour!", "Error");
                return;
            }

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

            _dialogService.ShowMessage("Tour log added successfully!", "Success");
        }
    }
}
