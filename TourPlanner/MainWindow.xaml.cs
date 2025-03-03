using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TourPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Tour> Tours { get; }
        private Tour? selectedTour = null;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Tours = 
            [
                new() { Description = "Hike through Vienna Woods", Date = DateTime.Now, Name = "Vienna Woods" },
                new() { Description = "Bike ride along the Danube", Date = DateTime.Now.AddDays(1), Name = "Danube Island" },
                new() { Description = "Explore historic landmarks", Date = DateTime.Now.AddDays(2), Name = "Vienna City Tour" },
                new() { Description = "Drive through the wine region", Date = DateTime.Now.AddDays(3), Name = "Wachau Valley" },
                new() { Description = "Mountain trek with scenic views", Date = DateTime.Now.AddDays(4), Name = "Alpine Adventure" }
            ];
        }
        private string newTourDesc = string.Empty;
        private string newTourName = string.Empty;
        public string NewTourDesc
        {
            get { return newTourDesc; }
            set
            {
                newTourDesc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewTourDesc)));
            }
        }

        public Tour? SelectedTour
        {
            get { return selectedTour; }
            set
            {
                selectedTour = value;
                Debug.WriteLine($"SelectedTask: {selectedTour?.Description}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTour)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTourAsList)));

            }
        }

        private ObservableCollection<Tour> selectedTourAsList = new ObservableCollection<Tour>();
        public ObservableCollection<Tour> SelectedTourAsList
        {
            get { return selectedTourAsList; }
            set
            {
                selectedTourAsList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTourAsList)));

            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedTour = e.AddedItems.Count > 0 ? e.AddedItems[0] as Tour : null;
        }

        public ICommand AddCommand => new RelayCommand(_ =>
        {
            Tours.Add(new Tour { Description = NewTourDesc, Date = DateTime.Now, Name = newTourName });
            NewTourDesc = string.Empty;
        }, _ => !string.IsNullOrEmpty(NewTourDesc));

        public ICommand DeleteCommand => new RelayCommand(param =>
        {
            if (SelectedTour != null)
            {
                Tours.Remove(SelectedTour);
            }
        }, _ => true);

        public ICommand ExitCommand => new RelayCommand(_ => Environment.Exit(0), _ => true);


        public event PropertyChangedEventHandler? PropertyChanged;
    }
}