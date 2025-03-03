using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner
{
    // Transport enum
    public enum TransportType
    {
        Bus, Train, Plane, Walk, Bicycle
    }
    public class Tour
    {
        
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public TransportType TransportType { get; set; }
        public double Distance { get; set; }
        public double EstimatedTime { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Name { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }
}
