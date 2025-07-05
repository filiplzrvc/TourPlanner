using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TourPlanner.BL.Services;
using TourPlanner.UI.Views;
using TourPlanner.Utilities;
using System.IO;
using System.Linq;
using TourPlanner.UI.Services;
using TourPlanner.Utilities.Logging;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.DependencyInjection;

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
            get => SelectedTour?.TourLogs ?? new ObservableCollection<TourLog>();
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

        public MainViewModel(TourService tourService, TourLogService tourLogService)
        {
            this.tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
            this.tourLogService = tourLogService ?? throw new ArgumentNullException(nameof(tourLogService));

            _dialogService= new DialogService();

            _logger.Info("MainViewModel initialized.");

            _ = LoadToursAsync();
        }

        private async Task LoadToursAsync()
        {
            var tours = await tourService.GetToursAsync();
            foreach (var tour in tours)
            {
                Tours.Add(tour);
            }
            ApplySearchFilter();
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
                    OnPropertyChanged(nameof(TourLogs)); // aktualisiere die Logs
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

            double avgTime = tour.TourLogs.Average(l => l.TotalTime);
            double avgDist = tour.TourLogs.Average(l => l.TotalDistance);
            double avgRating = tour.TourLogs.Average(l => l.Rating);

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


        public ICommand OpenAddTourWindowCommand => new RelayCommand(_ =>
        {
            _logger.Info("Opening Add Tour Window.");
            
            var tourService = App.AppHost.Services.GetRequiredService<TourService>();
            var addTourViewModel = new AddTourViewModel(tourService, Tours);
            var addTourWindow = new AddTourWindow(addTourViewModel); 
            
            ApplySearchFilter();
            _logger.Info("Closed Add Tour Window and refreshed tours.");
        });


 
        public ICommand OpenEditTourWindowCommand => new RelayCommand(_ =>
        {
            _logger.Info("Opening Edit Tour Window.");
            if (SelectedTour != null)
            {
                var tourService = App.AppHost.Services.GetRequiredService<TourService>();
                var editTourViewModel = new EditTourViewModel(tourService, SelectedTour);

                var editTourWindow = new EditTourWindow
                {
                    DataContext = editTourViewModel
                };

                editTourWindow.ShowDialog();
            }
            ApplySearchFilter();
            _logger.Info("Closed Edit Tour Window and refreshed tours.");
        }, _ => SelectedTour != null);
        
        


        public ICommand DeleteTourCommand => new RelayCommand(async _ =>
        {
            if (SelectedTour != null)
            {
                if (_dialogService.ConfirmDialog($"Are you sure you want to delete '{SelectedTour.Name}'?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    await tourService.DeleteTourAsync(SelectedTour);
                    Tours.Remove(SelectedTour);
                    _logger.Info($"Tour deleted: {SelectedTour.Name}");
                    SelectedTour = null;
                }
            }
            ApplySearchFilter();
        }, _ => SelectedTour != null);

        public ICommand OpenAddTourLogWindowCommand => new RelayCommand(_ =>
        {
             _logger.Info($"Opening AddTourLogWindow for tour: {SelectedTour?.Name}");
            if (SelectedTour == null)
            {
                _dialogService.ShowMessage("Please select a tour first.", "Error");
                return;
            }

            var tourLogService = App.AppHost.Services.GetRequiredService<TourLogService>();
            var addTourLogViewModel = new AddTourLogViewModel(tourLogService, SelectedTour.TourLogs, SelectedTour);

            var addTourLogWindow = new AddTourLogWindow
            {
                DataContext = addTourLogViewModel
            };

            addTourLogWindow.ShowDialog();
            ApplyTourLogSearchFilter(); 
             _logger.Info($"Closed AddTourLogWindow and refreshed tour logs.");
        });

        public ICommand OpenEditTourLogWindowCommand => new RelayCommand(_ =>
        {
            _logger.Info($"Opening EditTourLogWindow for tour: {SelectedTour?.Name}");
            if (SelectedTourLog == null)
            {
                _dialogService.ShowMessage("Please select a tour log to edit.", "Error");
                return;
            }

            var tourLogService = App.AppHost.Services.GetRequiredService<TourLogService>();
            var editTourLogViewModel = new EditTourLogViewModel(tourLogService, SelectedTourLog);

            var editTourLogWindow = new EditTourLogWindow
            {
              DataContext = editTourLogViewModel
            };
            editTourLogWindow.ShowDialog();
            ApplyTourLogSearchFilter(); 
            _logger.Info($"Closed EditTourLogWindow and refreshed tour logs.");
        }, _ => SelectedTourLog != null);
        
        
        public ICommand DeleteTourLogCommand => new RelayCommand(async _ =>
        {
            if (SelectedTourLog != null)
            {
                if (_dialogService.ConfirmDialog($"Are you sure you want to delete this log?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    await tourLogService.DeleteTourLogAsync(SelectedTourLog);
                    SelectedTour.TourLogs.Remove(SelectedTourLog);
                    _logger.Info($"Tour log Date([{SelectedTourLog.Date}]) deleted successfully.");
                    SelectedTourLog = null;
                }
            }
            ApplyTourLogSearchFilter();
        }, _ => SelectedTourLog != null);

        public ICommand GenerateReportCommand => new RelayCommand(_ =>
        {
            if (SelectedTour != null)
            {
                string fileName = $"{SelectedTour.Name}_Report.pdf".Replace(" ", "_");
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = System.IO.Path.Combine(desktop, fileName);

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
                string filePath = System.IO.Path.Combine(desktop, fileName);

                _logger.Info($"Generating PDF summary for tour: {SelectedTour.Name} at {filePath}");
                PdfReportGenerator.CreateSummaryReport(Tours, filePath);
                _dialogService.ShowMessage("Summary PDF report created successfully!", "Success");
                _logger.Info("PDF summary generated successfully.");
            }
        }, _ => Tours.Count > 0);

        public ICommand ImportToursCommand => new RelayCommand(async _ =>
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Import Tours"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _logger.Info($"Importing tours from {openFileDialog.FileName}");
                    var importedTours = TourDataSerializer.ImportTours(openFileDialog.FileName);

                    foreach (var tour in importedTours)
                    {
                        tour.Id = 0;

                        if (!string.IsNullOrEmpty(tour.Image))
                        {
                            string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tour.Image);
                            if (!File.Exists(absolutePath))
                            {
                                tour.Image = "../../Images/TestRoute.png";
                            }
                            else
                            {
                                tour.Image = absolutePath;
                            }
                        }

                        foreach (var log in tour.TourLogs)
                        {
                            log.Id = 0;
                            log.TourId = 0;
                            log.Tour = tour;
                        }

                        await tourService.AddTourAsync(tour);
                        Tours.Add(tour);
                    }

                    ApplySearchFilter();
                    _dialogService.ShowMessage("Tour data imported successfully.", "Import");
                    _logger.Info($"Imported {importedTours.Count} tours.");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"General import failed: {ex.InnerException?.Message ?? ex.Message}",
                                    "Error");
                    _logger.Error($"General import failed: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        });
 

    public ICommand ExportToursCommand => new RelayCommand(_ =>
    {
      try
      {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            Title = "Export Tours",
            FileName = "ToursExport.json"
        };
        if (SelectedTourLog == null)
        {
            _dialogService.ShowMessage("Please select a tour log first!", "Error");
            return;
        }

        if (saveFileDialog.ShowDialog() == true)
        {
             _logger.Info($"Exporting tours to {saveFileDialog.FileName}");
            TourDataSerializer.ExportTours(Tours, saveFileDialog.FileName);
            _dialogService.ShowMessage("Tour data exported successfully.", "Export");
            _logger.Info("Tours exported successfully.");
        }
       }catch(Exception ex)
        {
            _dialogService.ShowMessage($"Tour data failed to export {ex}", "Error");
            _logger.Error($"Tour data failed to export {ex}");
        }
    }, _ => Tours.Count > 0);


        public Brush DeleteTextColor
        {
            get => deleteTextColor;
            set
            {
                deleteTextColor = value;
                OnPropertyChanged(nameof(DeleteTextColor));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
