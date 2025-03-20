using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.Utilities;

namespace TourPlanner.UI.ViewModels
{
    public class AddTourViewModel
    {
        public string NewTourName { get; set; } = string.Empty;
        public string NewTourDesc { get; set; } = string.Empty;
        public string NewTourFrom { get; set; } = string.Empty;
        public string NewTourTo { get; set; } = string.Empty;
        public TransportType NewTourTransportType { get; set; } = TransportType.Walk;

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

        public ICommand ConfirmAddTourCommand { get; }

        public AddTourViewModel(TourService tourService, ObservableCollection<Tour> tours)
        {
            this.tourService = tourService;
            this.tours = tours;
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
            if(!CanAddTour())
            {
                MessageBox.Show("Please fill out all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var newTour = new Tour
            {
                Name = NewTourName,
                Description = NewTourDesc,
                From = NewTourFrom,
                To = NewTourTo,
                TransportType = NewTourTransportType
            };
            tourService.AddTour(newTour);
            tours.Add(newTour);

            MessageBox.Show("Tour added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Application.Current.Windows[1]?.Close();
        }

        
    }
}
