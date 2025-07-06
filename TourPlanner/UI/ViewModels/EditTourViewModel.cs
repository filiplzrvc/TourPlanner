using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.Utilities;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.ViewModels
{
    public class EditTourViewModel : INotifyPropertyChanged
    {
        private readonly TourService tourService;
        private readonly IDialogService _dialogService;
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();
        private readonly string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public Tour SelectedTour { get; }
        public Tour EditedTour { get; }

        private string? imagePath;
        public string? ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public string? RelativeImagePath { get; set; }

        public ICommand ConfirmEditTourCommand { get; }

        public ObservableCollection<TransportType> TransportTypes { get; } = new ObservableCollection<TransportType>
        {
            TransportType.Bus,
            TransportType.Train,
            TransportType.Plane,
            TransportType.Walk,
            TransportType.Bicycle
        };

        public ICommand SelectImageCommand => new RelayCommand(_ =>
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select Image"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                string fileName = Path.GetFileName(dialog.FileName);
                string destPath = Path.Combine(imagesFolder, fileName);

                int counter = 1;
                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);

                while (File.Exists(destPath))
                {
                    fileName = $"{name}_{counter++}{ext}";
                    destPath = Path.Combine(imagesFolder, fileName);
                }

                try
                {
                    File.Copy(dialog.FileName, destPath);

                    ImagePath = destPath;
                    RelativeImagePath = Path.Combine("Images", fileName); 
                    EditedTour.Image = RelativeImagePath;

                    _logger.Info($"Image selected and copied: {fileName}");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"Error copying image: {ex.Message}", "Error");
                    _logger.Error($"Error copying image: {ex}");
                }
            }
        });

        public EditTourViewModel(TourService tourService, Tour selectedTour)
        {
            this.tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
            SelectedTour = selectedTour ?? throw new ArgumentNullException(nameof(selectedTour));

            EditedTour = new Tour
            {
                Id = selectedTour.Id,
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

            InitializeCurrentImage();

            _dialogService = new DialogService();
            ConfirmEditTourCommand = new RelayCommand(async _ => await UpdateTourAsync());
        }

        private void InitializeCurrentImage()
        {
            if (!string.IsNullOrEmpty(SelectedTour.Image))
            {
                string currentImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SelectedTour.Image);
                if (File.Exists(currentImagePath))
                {
                    ImagePath = currentImagePath;
                    RelativeImagePath = SelectedTour.Image;
                }
                else
                {
                    ImagePath = null;
                    RelativeImagePath = "Images/TestRoute.png";
                    EditedTour.Image = RelativeImagePath;
                    _logger.Warn($"Image file not found: {currentImagePath}, using fallback");
                }
            }
            else
            {
                ImagePath = null;
                RelativeImagePath = "Images/TestRoute.png";
                EditedTour.Image = RelativeImagePath;
            }
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
                SelectedTour.Name = EditedTour.Name;
                SelectedTour.Description = EditedTour.Description;
                SelectedTour.From = EditedTour.From;
                SelectedTour.To = EditedTour.To;
                SelectedTour.TransportType = EditedTour.TransportType;
                SelectedTour.Distance = EditedTour.Distance;
                SelectedTour.EstimatedTime = EditedTour.EstimatedTime;
                SelectedTour.Date = EditedTour.Date;

                if (!string.IsNullOrWhiteSpace(RelativeImagePath))
                {
                    SelectedTour.Image = RelativeImagePath;
                }

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}