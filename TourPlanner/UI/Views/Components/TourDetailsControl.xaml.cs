using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using TourPlanner.BL.Services;
using TourPlanner.UI.ViewModels;
using TourPlanner.Utilities.Logging;

namespace TourPlanner.UI.Views.Components
{
    public partial class TourDetailsControl : UserControl
    {
        private static readonly ILoggerWrapper _logger = LoggerFactory.GetLogger();
        private bool _isMapInitialized = false;

        public TourDetailsControl()
        {
            InitializeComponent();
            _ = InitializeMapAsync();
            this.Loaded += OnControlLoaded;
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var mainViewModel = this.DataContext as MainViewModel;
            if (mainViewModel == null)
            {
                var mainWindow = App.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mainViewModel = mainWindow?.DataContext as MainViewModel;
            }

            if (mainViewModel != null)
            {
                mainViewModel.RouteUpdateRequested += OnRouteUpdateRequested;
                mainViewModel.MapClearRequested += OnMapClearRequested;
            }
        }

        private async void OnRouteUpdateRequested(string from, string to)
        {
            await ShowRouteAsync(from, to);
        }

        private async void OnMapClearRequested()
        {
            await ClearMapAsync();
        }

        private async Task InitializeMapAsync()
        {
            try
            {
                await LeafletMap.EnsureCoreWebView2Async();

                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web", "map.html");

                if (!File.Exists(htmlPath))
                {
                    _logger.Error($"Map HTML file not found at: {htmlPath}");
                    return;
                }

                htmlPath = htmlPath.Replace("\\", "/");
                LeafletMap.Source = new Uri($"file:///{htmlPath}");

                LeafletMap.NavigationCompleted += (sender, e) =>
                {
                    _isMapInitialized = true;
                    _logger.Info("Leaflet map initialized successfully");
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"Error initializing map: {ex.Message}");
            }
        }

        public async Task ShowRouteAsync(string from, string to)
        {
            try
            {
                if (!_isMapInitialized)
                {
                    await Task.Delay(1000);
                    if (!_isMapInitialized) return;
                }

                var orsClient = App.AppHost.Services.GetRequiredService<OpenRouteServiceClient>();
                var (distance, duration, geoJson, fromCoords, toCoords) =
                    await orsClient.GetRouteWithCoordinatesAsync(from, to);

                var fromLatStr = fromCoords.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                var fromLonStr = fromCoords.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                var toLatStr = toCoords.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                var toLonStr = toCoords.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);

                var escapedGeoJson = geoJson.Replace("\"", "\\\"");

                string script = $"loadMap(\"{escapedGeoJson}\", {fromLatStr}, {fromLonStr}, {toLatStr}, {toLonStr})";
                await LeafletMap.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error showing route: {ex.Message}");

                try
                {
                    await ShowMarkersOnlyAsync(from, to);
                }
                catch (Exception fallbackEx)
                {
                    _logger.Error($"Fallback also failed: {fallbackEx.Message}");
                }
            }
        }

        private async Task ShowMarkersOnlyAsync(string from, string to)
        {
            try
            {
                var orsClient = App.AppHost.Services.GetRequiredService<OpenRouteServiceClient>();
                var (_, _, _, fromCoords, toCoords) =
                    await orsClient.GetRouteWithCoordinatesAsync(from, to);

                var fromLatStr = fromCoords.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                var fromLonStr = fromCoords.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                var toLatStr = toCoords.Latitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
                var toLonStr = toCoords.Longitude.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);

                string script = $"showMarkersOnly({fromLatStr}, {fromLonStr}, {toLatStr}, {toLonStr})";
                await LeafletMap.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error showing markers only: {ex.Message}");
            }
        }

        public async Task ClearMapAsync()
        {
            try
            {
                if (!_isMapInitialized) return;

                string clearScript = @"
                    try {
                        clearAll();
                        map.setView([47.0707, 15.4395], 7);
                        'success';
                    } catch (error) {
                        'error: ' + error.message;
                    }
                ";

                await LeafletMap.ExecuteScriptAsync(clearScript);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error clearing map: {ex.Message}");
            }
        }
    }
}