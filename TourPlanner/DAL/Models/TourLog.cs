using System;
using System.ComponentModel;

namespace TourPlanner
{
    public class TourLog : INotifyPropertyChanged
    {
        public int Id { get; set; }  // Primärschlüssel

        private DateTime date = DateTime.UtcNow;
        private string comment = string.Empty;
        private int difficulty;
        private double totalDistance;
        private double totalTime;
        private int rating;

        // 🆕 FK + Navigation
        public int TourId { get; set; }
        public Tour Tour { get; set; }

        public DateTime Date { get => date; set => SetProperty(ref date, value); }
        public string Comment { get => comment; set => SetProperty(ref comment, value); }
        public int Difficulty { get => difficulty; set => SetProperty(ref difficulty, value); }
        public double TotalDistance { get => totalDistance; set => SetProperty(ref totalDistance, value); }
        public double TotalTime { get => totalTime; set => SetProperty(ref totalTime, value); }
        public int Rating { get => rating; set => SetProperty(ref rating, value); }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool SetProperty<T>(ref T field, T value, string propertyName = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}