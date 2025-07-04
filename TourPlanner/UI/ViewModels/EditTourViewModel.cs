using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.Utilities;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.ViewModels
{
    public class EditTourViewModel
    {
        private readonly TourService tourService;
        private readonly IDialogService _dialogService;
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();


        public Tour SelectedTour { get; }
        public Tour EditedTour { get; }

        public ICommand ConfirmEditTourCommand { get; }

        public ObservableCollection<TransportType> TransportTypes { get; } = new ObservableCollection<TransportType>
        {
            TransportType.Bus,
            TransportType.Train,
            TransportType.Plane,
            TransportType.Walk,
            TransportType.Bicycle
        };

        public EditTourViewModel(TourService tourService, Tour selectedTour)
        {
            this.tourService = tourService;
            SelectedTour = selectedTour;

            EditedTour = new Tour
            {
                Name = selectedTour.Name,
                Description = selectedTour.Description,
                From = selectedTour.From,
                To = selectedTour.To,
                TransportType = selectedTour.TransportType,
                Distance = selectedTour.Distance,
                EstimatedTime = selectedTour.EstimatedTime,
                Date = selectedTour.Date,
                Image = selectedTour.Image
            };  
            _dialogService= new DialogService();

            ConfirmEditTourCommand = new RelayCommand(_ => UpdateTour()); // _ => CanUpdateTour() hinzufügen?
        }

        private bool CanUpdateTour()
        {
            return !string.IsNullOrWhiteSpace(EditedTour.Name) &&
                   !string.IsNullOrWhiteSpace(EditedTour.Description) &&
                   !string.IsNullOrWhiteSpace(EditedTour.From) &&
                   !string.IsNullOrWhiteSpace(EditedTour.To);
        }

        private void UpdateTour()
        {
            if (!CanUpdateTour())
            {
                _dialogService.ShowMessage("Please fill out all fields!", "Error");
                return;
            }

            try
            {
                SelectedTour.Name = EditedTour.Name;
                SelectedTour.Description = EditedTour.Description;
                SelectedTour.From = EditedTour.From;
                SelectedTour.To = EditedTour.To;
                SelectedTour.TransportType = EditedTour.TransportType;
                SelectedTour.Distance = EditedTour.Distance;
                SelectedTour.EstimatedTime = EditedTour.EstimatedTime;
                SelectedTour.Date = EditedTour.Date;
                SelectedTour.Image = EditedTour.Image;

                tourService.UpdateTour(SelectedTour);
                _dialogService.ShowMessage($"Tour [{SelectedTour.Name}] updated successfully!", "Success");
                _dialogService.ShowMessage("Tour updated successfully!", "Success");

                CloseWindow();
            }catch(Exception ex)
            {
                _logger.Error($"Error while updating tour: {ex}");
            }

        }

        protected virtual void CloseWindow()
        {
            if (Application.Current != null && Application.Current.Windows.Count > 1)
            {
                Application.Current.Windows[1]?.Close();
            }
        }
    }
}
