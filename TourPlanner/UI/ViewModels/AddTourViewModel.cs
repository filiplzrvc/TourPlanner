using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.Utilities;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.ViewModels
{
    public class AddTourViewModel : INotifyPropertyChanged
    {
        public string NewTourName { get; set; } = string.Empty;
        public string NewTourDesc { get; set; } = string.Empty;
        public string NewTourFrom { get; set; } = string.Empty;
        public string NewTourTo { get; set; } = string.Empty;
        public TransportType NewTourTransportType { get; set; } = TransportType.Walk;

        public string ImagePath { get; set; } = "../../Images/TestRoute.png";
        private readonly string imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        private readonly IDialogService _dialogService;

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

        public string? RelativeImagePath { get; set; } // für die DB

        private readonly string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        public ObservableCollection<TransportType> TransportTypes { get; } = new ObservableCollection<TransportType>
        {
            TransportType.Bus,
            TransportType.Train,
            TransportType.Plane,
            TransportType.Walk,
            TransportType.Bicycle
        };

        private readonly TourService tourService;
        private readonly ObservableCollection<Tour> tours;
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand SelectImageCommand => new RelayCommand(_ =>
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select Image"
            };

            if (dialog.ShowDialog() == true)
            {
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

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

                File.Copy(dialog.FileName, destPath);

                
                ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName); // Absoluter Pfad für WPF
                RelativeImagePath = Path.Combine("Images", fileName); // Relativer Pfad für die DB
            }
        });


        public ICommand ConfirmAddTourCommand { get; }

        public AddTourViewModel(TourService tourService, ObservableCollection<Tour> tours)
        {
            this.tourService = tourService;
            this.tours = tours;
            _dialogService = new DialogService();
            ConfirmAddTourCommand = new RelayCommand(async _ => await AddTourAsync());
        }

        private bool CanAddTour()
        {
            return !string.IsNullOrWhiteSpace(NewTourName) &&
                   !string.IsNullOrWhiteSpace(NewTourDesc) &&
                   !string.IsNullOrWhiteSpace(NewTourFrom) &&
                   !string.IsNullOrWhiteSpace(NewTourTo);
        }

        private async Task AddTourAsync()
        {
            if (!CanAddTour())
            {
                _dialogService.ShowMessage("Please fill out all fields!", "Error");
                _logger.Warn("AddTour failed: Missing required fields.");
                return;
            }
            try
            {
                var newTour = new Tour
                {
                    Name = NewTourName?.Trim() ?? "Unnamed Tour",
                    Description = NewTourDesc?.Trim() ?? "No description",
                    From = NewTourFrom?.Trim() ?? "Unknown",
                    To = NewTourTo?.Trim() ?? "Unknown",
                    TransportType = NewTourTransportType,
                    Distance = 0,
                    EstimatedTime = 0,
                    Image = string.IsNullOrWhiteSpace(RelativeImagePath)
                        ? "Images/TestRoute.png" // Default-Bild
                        : RelativeImagePath,
                    Date = DateTime.UtcNow
                };
                await tourService.AddTourAsync(newTour);
                tours.Add(newTour);
                _logger.Info($"Tour added successfully: {newTour.Name}");
                _dialogService.ShowMessage("Tour added successfully!", "Success");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Failed to add tour: {ex.Message}", "Error");
                 _logger.Error($"Error while adding tour: {ex}");
            }
            _dialogService.CloseCurrentWindow();
        }
    }
}
