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
                new() { Description = "Hike through Vienna Woods", Date = DateTime.Now, Name = "Vienna Woods", Image="https://www.datocms-assets.com/50871/1716498222-final-2024-ticketoffice-map-95-75x59-2156x1328.jpg"},
                new() { Description = "Bike ride along the Danube", Date = DateTime.Now.AddDays(1), Name = "Danube Island", Image="https://www.chamrousse.com/medias/images/info_pages/multitailles/800x600_plan-randonnees-balade-chamrousse-ete-2024-3428.jpg" },
                new() { Description = "Explore historic landmarks", Date = DateTime.Now.AddDays(2), Name = "Vienna City Tour", Image="https://www.pferde-stadlpaura.at/images/News/7542.jpg" },
                new() { Description = "Drive through the wine region", Date = DateTime.Now.AddDays(3), Name = "Wachau Valley", Image="https://www.fnp.de/assets/images/27/132/27132087-der-plan-zeigt-die-neuen-routen-wanderung-vier-ist-nicht-eingezeichnet-da-sie-sich-durchs-dorf-schlaengelt-sie-dauert-rund-90-minuten-und-bietet-viele-1Pfe.jpg" },
                new() { Description = "Mountain trek with scenic views", Date = DateTime.Now.AddDays(4), Name = "Alpine Adventure", Image="https://i0.wp.com/limberlostforest.com/wp-content/uploads/2023/07/HikingMap09-06-2023.v2_page-0001.jpg?resize=900%2C636&ssl=1" }
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
                Debug.WriteLine($"SelectedTour: {selectedTour?.Description}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTour)));

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