using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace TourPlanner
{
    // Transport enum
    public enum TransportType
    {
        Bus, Train, Plane, Walk, Bicycle
    }
    public class Tour : INotifyPropertyChanged
    {
        public int Id { get; set; }  // Primärschlüssel
        public ObservableCollection<TourLog> TourLogs { get; set; } = new ObservableCollection<TourLog>();

        
        private string name = "Unnamed Tour";
        private string description = "No description available";
        private string from = "Unknown";
        private string to = "Unknown";
        private TransportType transportType;
        private double distance;
        private double estimatedTime;
        private DateTime date = DateTime.UtcNow;
        private string image = "../../Images/TestRoute.png";

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Description { get => description; set => SetProperty(ref description, value); }
        public string From { get => from; set => SetProperty(ref from, value); }
        public string To { get => to; set => SetProperty(ref to, value); }
        public TransportType TransportType { get => transportType; set => SetProperty(ref transportType, value); }
        public double Distance { get => distance; set => SetProperty(ref distance, value); }
        public double EstimatedTime { get => estimatedTime; set => SetProperty(ref estimatedTime, value); }
        public DateTime Date { get => date; set => SetProperty(ref date, value); }
        public string? Image { get => image; set => SetProperty(ref image, value); }


        [JsonIgnore]
        public BitmapImage TourImage
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Image))
                    {
                        string absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Image);

                        if (File.Exists(absolutePath))
                        {
                            return new BitmapImage(new Uri(absolutePath, UriKind.Absolute));
                        }
                    }
                }
                catch { }

                // 🛡️ Fallback-Bild
                return new BitmapImage(new Uri("pack://application:,,,/Images/TestRoute.png"));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool SetProperty<T>(ref T field, T value, string propertyName = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
