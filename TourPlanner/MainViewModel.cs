using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace TourPlanner
{
    public class MainViewModel : INotifyPropertyChanged
    {
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

        public ObservableCollection<Tour> Tours { get; }

        private Tour? selectedTour = null;
        public Tour? SelectedTour
        {
            get { return selectedTour; }
            set
            {
                selectedTour = value;
                Debug.WriteLine($"SelectedTask: {selectedTour?.Description}");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTour)));
            }
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


        public MainViewModel()
        {
            Tours =
            [
                new() { Description = "Hike through Vienna Woods", Date = DateTime.Now, Name = "Vienna Woods" },
                new() { Description = "Bike ride along the Danube", Date = DateTime.Now.AddDays(1), Name = "Danube Island" },
                new() { Description = "Explore historic landmarks", Date = DateTime.Now.AddDays(2), Name = "Vienna City Tour" },
                new() { Description = "Drive through the wine region", Date = DateTime.Now.AddDays(3), Name = "Wachau Valley" },
                new() { Description = "Mountain trek with scenic views", Date = DateTime.Now.AddDays(4), Name = "Alpine Adventure" }
            ];

        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
