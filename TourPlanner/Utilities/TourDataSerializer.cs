using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace TourPlanner.Utilities
{
    public static class TourDataSerializer
    {
        public static void ExportTours(IEnumerable<Tour> tours, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            foreach (var tour in tours)
            {
                foreach (var log in tour.TourLogs)
                {
                    log.Tour = null;
                }
            }

            try
            {
                string json = JsonSerializer.Serialize(tours, options);
                File.WriteAllText(filePath, json);

                MessageBox.Show("Export erfolgreich!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Export: {ex.Message}", "Export Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<Tour> ImportTours(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            try
            {
                string json = File.ReadAllText(filePath);
                var importedTours = JsonSerializer.Deserialize<List<Tour>>(json, options) ?? new List<Tour>();

                foreach (var tour in importedTours)
                {
                    tour.Id = 0;

                    foreach (var log in tour.TourLogs)
                    {
                        log.Id = 0;
                        log.TourId = 0;
                        log.Tour = tour;
                    }
                }

                return importedTours;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Import: {ex.Message}", "Import Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Tour>();
            }
        }
    }
}
