using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TourPlanner.Utilities
{
    public static class TourDataSerializer
    {
        public static void ExportTours(IEnumerable<Tour> tours, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(tours, options);
            File.WriteAllText(filePath, json);
        }

        public static List<Tour> ImportTours(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Tour>>(json) ?? new List<Tour>();
        }
    }
}
