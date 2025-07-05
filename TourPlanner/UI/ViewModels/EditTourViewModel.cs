using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Utilities;

namespace TourPlanner.UI.ViewModels
{
    public class EditTourViewModel
    {
        private readonly TourService tourService;
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
                MessageBox.Show("Please fill out all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            try
            {
                await tourService.UpdateTourAsync(SelectedTour);

                MessageBox.Show("Tour updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update tour: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected virtual void CloseWindow()
        {
            if (Application.Current.Windows.Count > 1)
            {
                Application.Current.Windows[1]?.Close();
            }
        }
    }
}
