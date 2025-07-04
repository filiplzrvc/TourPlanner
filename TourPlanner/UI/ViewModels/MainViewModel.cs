using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner;
using TourPlanner.Utilities;
using TourPlanner.UI.Views;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Linq;
using TourPlanner.UI.Services;
using TourPlanner.Utilities.Logging;
using log4net.Repository.Hierarchy;

namespace TourPlanner.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Tour? selectedTour;
        private TourLog? selectedTourLog;
        private Brush deleteTextColor = Brushes.Black;
        private readonly TourService tourService;
        private readonly TourLogService tourLogService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Tour> Tours { get; } = new();
        public ObservableCollection<TourLog> TourLogs
        {
            get => SelectedTour?.TourLogs ?? new ObservableCollection<TourLog> (); 
        }

        public ObservableCollection<Tour> FilteredTours { get; } = new();
        public ObservableCollection<TourLog> FilteredTourLogs { get; } = new();


        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();

        private string searchQuery = string.Empty;
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                if (searchQuery != value)
                {
                    searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    ApplySearchFilter();
                }
            }
        }

        private string tourLogSearchQuery = string.Empty;
        public string TourLogSearchQuery
        {
            get => tourLogSearchQuery;
            set
            {
                if (tourLogSearchQuery != value)
                {
                    tourLogSearchQuery = value;
                    OnPropertyChanged(nameof(TourLogSearchQuery));
                    ApplyTourLogSearchFilter();
                }
            }
        }

        public MainViewModel()
        {
            tourService = new TourService();
            tourLogService = new TourLogService();
            _dialogService= new DialogService();

            _logger.Info("MainViewModel initialized.");

            foreach (var tour in tourService.GetTours())
            {
                Tours.Add(tour);
            }

            foreach (var log in tourLogService.GetTourLogs())
            {
                TourLogs.Add(log);
            }
            ApplySearchFilter();
            ApplyTourLogSearchFilter();

        }

        private bool MatchesTourLogSearch(TourLog log, string lowerSearch)
        {
            return log.Comment.ToLower().Contains(lowerSearch) ||
                   log.Date.ToString("g").ToLower().Contains(lowerSearch) ||
                   log.Difficulty.ToString().ToLower().Contains(lowerSearch) ||
                   log.TotalDistance.ToString().ToLower().Contains(lowerSearch) ||
                   log.TotalTime.ToString().ToLower().Contains(lowerSearch) ||
                   log.Rating.ToString().ToLower().Contains(lowerSearch);
        }

        private bool MatchesComputedFields(Tour tour, string lowerSearch)
        {
            if (!tour.TourLogs.Any())
                return false;

            double avgTime = tour.TourLogs.Any() ? tour.TourLogs.Average(l => l.TotalTime) : 0.0;
            double avgDist = tour.TourLogs.Any() ? tour.TourLogs.Average(l => l.TotalDistance) : 0.0;
            double avgRating = tour.TourLogs.Any() ? tour.TourLogs.Average(l => l.Rating) : 0.0;

            return avgTime.ToString().ToLower().Contains(lowerSearch) ||
                   avgDist.ToString().ToLower().Contains(lowerSearch) ||
                   avgRating.ToString().ToLower().Contains(lowerSearch);
        }

        private void ApplySearchFilter()
        {
            FilteredTours.Clear();

            var lowerSearch = SearchQuery.ToLower();

            foreach (var tour in Tours)
            {
                bool inBasicFields =
                    tour.Name.ToLower().Contains(lowerSearch) ||
                    tour.Description.ToLower().Contains(lowerSearch) ||
                    tour.From.ToLower().Contains(lowerSearch) ||
                    tour.To.ToLower().Contains(lowerSearch) ||
                    tour.TransportType.ToString().ToLower().Contains(lowerSearch);

                bool inComputedFields = MatchesComputedFields(tour, lowerSearch);
                bool inTourLogs = tour.TourLogs.Any(log => MatchesTourLogSearch(log, lowerSearch));

                if (inBasicFields || inComputedFields || inTourLogs)
                {
                    FilteredTours.Add(tour);
                }
            }

            if (FilteredTours.Count == 0)
                SelectedTour = null;
        }

        private void ApplyTourLogSearchFilter()
        {
            FilteredTourLogs.Clear();

            if (SelectedTour == null)
                return;

            var lowerSearch = TourLogSearchQuery.ToLower();

            foreach (var log in SelectedTour.TourLogs)
            {
                if (MatchesTourLogSearch(log, lowerSearch))
                {
                    FilteredTourLogs.Add(log);
                }
            }
        }

        public ICommand GenerateReportCommand => new RelayCommand(_ =>
        {
            if (SelectedTour != null)
            {
                string fileName = $"{SelectedTour.Name}_Report.pdf".Replace(" ", "_");
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktop, fileName);

                _logger.Info($"Generating PDF report for tour: {SelectedTour.Name} at {filePath}");
                PdfReportGenerator.CreateSingleTourReport(SelectedTour, filePath);
                _dialogService.ShowMessage("PDF report was created successfully!", "Success");
                _logger.Info("PDF report generated successfully.");
            }
        }, _ => SelectedTour != null);


        public ICommand GenerateSummaryReportCommand => new RelayCommand(_ =>
        {
            if (Tours.Count > 0)
            {
                string fileName = $"Tour_Summary_Report.pdf";
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = Path.Combine(desktop, fileName);

                _logger.Info($"Generating PDF summary for tour: {SelectedTour.Name} at {filePath}");
                PdfReportGenerator.CreateSummaryReport(Tours, filePath);
                _dialogService.ShowMessage("Summary PDF report created successfully!", "Success");
                _logger.Info("PDF summary generated successfully.");

            }
        }, _ => Tours.Count > 0);

        public ICommand ExportToursCommand => new RelayCommand(_ =>
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON-Dateien (*.json)|*.json",
                FileName = "tours_export.json",
                DefaultExt = ".json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                _logger.Info($"Exporting tours to {saveDialog.FileName}");
                TourDataSerializer.ExportTours(Tours, saveDialog.FileName);
                _dialogService.ShowMessage("Tour data exported successfully.", "Export");
                _logger.Info("Tours exported successfully.");

            }
        }, _ => Tours.Count > 0);

        public ICommand ImportToursCommand => new RelayCommand(_ =>
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON-Dateien (*.json)|*.json"
            };

            if (openDialog.ShowDialog() == true)
            {
                _logger.Info($"Importing tours from {openDialog.FileName}");
                var importedTours = TourDataSerializer.ImportTours(openDialog.FileName);
                foreach (var tour in importedTours)
                {
                    Tours.Add(tour);
                }
                _dialogService.ShowMessage("Tour data imported successfully.", "Import");
                _logger.Info($"Imported {importedTours.Count} tours.");
            }
            ApplySearchFilter();
        });

        public Tour? SelectedTour
        {
            get => selectedTour;
            set
            {
                if (selectedTour != value)
                {
                    selectedTour = value;
                    OnPropertyChanged(nameof(SelectedTour));
                    ApplyTourLogSearchFilter();
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
            _logger.Info("Opening Add Tour Window.");
            var window = new AddTourWindow
            {
                DataContext = new AddTourViewModel(tourService, Tours)
            };
            window.ShowDialog();
            ApplySearchFilter();
            _logger.Info("Closed Add Tour Window and refreshed tours.");

        });
      
        public ICommand OpenEditTourWindowCommand => new RelayCommand(_ =>
        {
            _logger.Info("Opening Edit Tour Window.");
            if (SelectedTour != null)
            {
                var window = new EditTourWindow
                {
                    DataContext = new EditTourViewModel(tourService, SelectedTour)
                };
                window.ShowDialog();
            }
            ApplySearchFilter();
            _logger.Info("Closed Edit Tour Window and refreshed tours.");
        }, _ => SelectedTour != null);


        public ICommand DeleteTourCommand => new RelayCommand(_ =>
        {
            if (SelectedTour != null)
            {
                tourService.DeleteTour(SelectedTour);
                Tours.Remove(SelectedTour);
                _logger.Info($"Tour deleted: {SelectedTour.Name}");
                SelectedTour = null;
            }
            ApplySearchFilter();
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

            _logger.Info($"Opening AddTourLogWindow for tour: {SelectedTour?.Name}");
            var window = new AddTourLogWindow();

            if (SelectedTour == null)
            {
                _dialogService.ShowMessage("Please select a tour first!", "Error");
                return;
            }
            window.DataContext = new AddTourLogViewModel(tourLogService, SelectedTour.TourLogs);
            window.ShowDialog();
            DeleteTextColor = Brushes.White;
            ApplyTourLogSearchFilter();
            _logger.Info($"Closed AddTourLogWindow and refreshed tour logs.");
        });

        public ICommand OpenEditTourLogWindowCommand => new RelayCommand(_ =>
        {
            _logger.Info($"Opening EditTourLogWindow for tour: {SelectedTour?.Name}");
            if (SelectedTourLog == null)
            {
                _dialogService.ShowMessage("Please select a tour log first!", "Error");
                return;
            }

            var window = new EditTourLogWindow
            {
                DataContext = new EditTourLogViewModel(tourLogService, SelectedTourLog)
            };
            window.ShowDialog();
            ApplyTourLogSearchFilter();
            _logger.Info($"Closed EditTourLogWindow and refreshed tour logs.");

        }, _ => SelectedTourLog != null);

        public ICommand DeleteTourLogCommand => new RelayCommand(_ =>
        {
            if (SelectedTourLog != null)
            {
                tourLogService.DeleteTourLog(SelectedTourLog);
                TourLogs.Remove(SelectedTourLog);
                _logger.Info($"Tour log Date([{SelectedTourLog.Date}]) deleted successfully.");
                SelectedTourLog = null;
            }
            ApplyTourLogSearchFilter();
        }, _ => SelectedTourLog != null);



        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
