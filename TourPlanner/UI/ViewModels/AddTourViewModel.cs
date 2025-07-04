using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Services;
using TourPlanner.Utilities;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.ViewModels
{
    public class AddTourViewModel:INotifyPropertyChanged
    {
        public string NewTourName { get; set; } = string.Empty;
        public string NewTourDesc { get; set; } = string.Empty;
        public string NewTourFrom { get; set; } = string.Empty;
        public string NewTourTo { get; set; } = string.Empty;
        public TransportType NewTourTransportType { get; set; } = TransportType.Walk;
        public string ImagePath { get; set; } = "../../Images/TestRoute.png";
        private readonly string imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        private readonly IDialogService _dialogService;


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
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select Image"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                string fileName = Path.GetFileName(dialog.FileName);
                string destPath = Path.Combine(imageFolder, fileName);

                int counter = 1;
                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);

                while (File.Exists(destPath))
                {
                    fileName = $"{name}_{counter++}{ext}";
                    destPath = Path.Combine(imageFolder, fileName);
                }
                string relative = Path.Combine("Images", fileName);
                ImagePath = Path.GetFullPath(relative);
                OnPropertyChanged(nameof(ImagePath)); 
            }
        });



        public ICommand ConfirmAddTourCommand { get; }

        public AddTourViewModel(TourService tourService, ObservableCollection<Tour> tours)
        {
            this.tourService = tourService;
            this.tours = tours;
            _dialogService = new DialogService();
            ConfirmAddTourCommand = new RelayCommand(_ => AddTour()); //_ => CanAddTour() hinzufügen?
        }

        private bool CanAddTour()
        {
            return !string.IsNullOrWhiteSpace(NewTourName) &&
                   !string.IsNullOrWhiteSpace(NewTourDesc) &&
                   !string.IsNullOrWhiteSpace(NewTourFrom) && 
                   !string.IsNullOrWhiteSpace(NewTourTo);
        }

        private void AddTour()
        {
            _logger.Debug("AddTour command executed.");
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
                    Name = NewTourName,
                    Description = NewTourDesc,
                    From = NewTourFrom,
                    To = NewTourTo,
                    TransportType = NewTourTransportType,
                    Image = ImagePath
                };
                tourService.AddTour(newTour);
                tours.Add(newTour);
                _logger.Info($"Tour added successfully: {newTour.Name}");
                _dialogService.ShowMessage("Tour added successfully!", "Success");
                CloseWindow();
            }catch(Exception ex)
            {
                _logger.Error($"Error while adding tour: {ex}");
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
