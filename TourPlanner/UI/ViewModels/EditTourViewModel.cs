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
            this.tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
            SelectedTour = selectedTour ?? throw new ArgumentNullException(nameof(selectedTour));

            // Kopie erstellen, damit Änderungen erst nach Bestätigung übernommen werden
            EditedTour = new Tour
            {
                Id = selectedTour.Id, // wichtig: Id mitgeben, sonst weiß EF nicht welches Entity
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

            ConfirmEditTourCommand = new RelayCommand(async _ => await UpdateTourAsync());
        }

        private bool CanUpdateTour()
        {
            return !string.IsNullOrWhiteSpace(EditedTour.Name) &&
                   !string.IsNullOrWhiteSpace(EditedTour.Description) &&
                   !string.IsNullOrWhiteSpace(EditedTour.From) &&
                   !string.IsNullOrWhiteSpace(EditedTour.To);
        }

        private async Task UpdateTourAsync()
        {
            if (!CanUpdateTour())
            {
                _dialogService.ShowMessage("Please fill out all fields!", "Error");
                return;
            }

            try
            {
                // Änderungen von EditedTour ins Original übernehmen
                SelectedTour.Name = EditedTour.Name;
                SelectedTour.Description = EditedTour.Description;
                SelectedTour.From = EditedTour.From;
                SelectedTour.To = EditedTour.To;
                SelectedTour.TransportType = EditedTour.TransportType;
                SelectedTour.Distance = EditedTour.Distance;
                SelectedTour.EstimatedTime = EditedTour.EstimatedTime;
                SelectedTour.Date = EditedTour.Date;
                SelectedTour.Image = EditedTour.Image;
              
                await tourService.UpdateTourAsync(SelectedTour);

                _dialogService.ShowMessage("Tour updated successfully!", "Success");
                _logger.Info($"Tour [{SelectedTour.Name}] updated successfully!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Failed to update tour: {ex.Message}", "Error");
              _logger.Error($"Error while updating tour: {ex}");
            }
            _dialogService.CloseCurrentWindow();
          
        }
    }
}
