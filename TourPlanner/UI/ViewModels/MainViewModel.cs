using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner;
using TourPlanner.Utilities;
using TourPlanner.UI.Views;
using System.Windows;
using System.Windows.Media;

namespace TourPlanner.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Tour? selectedTour;
        private TourLog? selectedTourLog;
        private Brush deleteTextColor = Brushes.Black;
        private readonly TourService tourService;
        private readonly TourLogService tourLogService;

        public ObservableCollection<Tour> Tours { get; } = new();
        public ObservableCollection<TourLog> TourLogs
        {
            get => SelectedTour?.TourLogs ?? new ObservableCollection<TourLog> (); 
        }


        private string newTourName = string.Empty;
        private string newTourDesc = string.Empty;

        public MainViewModel()
        {
            tourService = new TourService();
            tourLogService = new TourLogService();

            foreach (var tour in tourService.GetTours())
            {
                Tours.Add(tour);
            }

            foreach (var log in tourLogService.GetTourLogs())
            {
                TourLogs.Add(log);
            }
        }

        public Tour? SelectedTour
        {
            get => selectedTour;
            set
            {
                if (selectedTour != value)
                {
                    selectedTour = value;
                    OnPropertyChanged(nameof(SelectedTour));
                }
            }
        }

        public TourLog? SelectedTourLog
        {
            get => selectedTourLog;
            set
            {
                if (selectedTourLog != value)
                {
                    selectedTourLog = value;
                    OnPropertyChanged(nameof(SelectedTourLog));
                }
            }
        }

        public ICommand OpenAddTourWindowCommand => new RelayCommand(_ =>
        {
            var window = new AddTourWindow
            {
                DataContext = new AddTourViewModel(tourService, Tours)
            };
            window.ShowDialog();
        });


        public ICommand OpenEditTourWindowCommand => new RelayCommand(_ =>
        {
            if (SelectedTour != null)
            {
                var window = new EditTourWindow
                {
                    DataContext = new EditTourViewModel(tourService, SelectedTour)
                };
                window.ShowDialog();
            }
        }, _ => SelectedTour != null);


        public ICommand DeleteTourCommand => new RelayCommand(_ =>
        {
            if (SelectedTour != null)
            {
                tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }, _ => SelectedTour != null);

        public Brush DeleteTextColor
        {
            get => deleteTextColor;
            set
            {
                deleteTextColor = value;
                OnPropertyChanged(nameof(DeleteTextColor));
            }
        }
        public ICommand OpenAddTourLogWindowCommand => new RelayCommand(_ =>
        {
            var window = new AddTourLogWindow();

            if (SelectedTour == null)
            {
                MessageBox.Show("Please select a tour first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            window.DataContext = new AddTourLogViewModel(tourLogService, SelectedTour.TourLogs);
            window.ShowDialog();
            DeleteTextColor = Brushes.White;

        });

        public ICommand OpenEditTourLogWindowCommand => new RelayCommand(_ =>
        {
            if (SelectedTourLog == null)
            {
                MessageBox.Show("Please select a tour log first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

                var window = new EditTourLogWindow
                {
                    DataContext = new EditTourLogViewModel(tourLogService, SelectedTourLog)
                };
                window.ShowDialog();
            
        }, _ => SelectedTourLog != null);

        public ICommand DeleteTourLogCommand => new RelayCommand(_ =>
        {
            if (SelectedTourLog != null)
            {
                tourLogService.DeleteTourLog(SelectedTourLog);
                TourLogs.Remove(SelectedTourLog);
                SelectedTourLog = null;
            }
        }, _ => SelectedTourLog != null);


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
