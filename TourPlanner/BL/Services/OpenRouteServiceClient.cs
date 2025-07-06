using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace TourPlanner.BL.Services
{
    public class OpenRouteServiceClient
    {
        private readonly string apiKey;
        private readonly HttpClient httpClient;

        public OpenRouteServiceClient(string apiKey)
        {
            this.apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openrouteservice.org")
            };
        }

        public async Task<(double Distance, double Duration, string GeoJson, (double Latitude, double Longitude) FromCoords, (double Latitude, double Longitude) ToCoords)>
            GetRouteWithCoordinatesAsync(string from, string to)
        {
            var fromCoords = await GeocodeAsync(from);
            var toCoords = await GeocodeAsync(to);

            if (fromCoords == null || toCoords == null)
                throw new Exception("Could not geocode one or both locations.");

            var routeRequest = new
            {
                coordinates = new[]
                {
                    new[] { fromCoords.Value.Longitude, fromCoords.Value.Latitude },
                    new[] { toCoords.Value.Longitude, toCoords.Value.Latitude }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(routeRequest));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "/v2/directions/driving-car/geojson")
            {
                Content = content
            };
            request.Headers.Add("Authorization", apiKey);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);

            var distance = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary")
                .GetProperty("distance")
                .GetDouble() / 1000.0; // m ➝ km

            var duration = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary")
                .GetProperty("duration")
                .GetDouble() / 3600.0; // s ➝ h

            return (distance, duration, jsonResponse, fromCoords.Value, toCoords.Value);
        }

        private async Task<(double Latitude, double Longitude)?> GeocodeAsync(string location)
        {
            var requestUri = $"/geocode/search?text={Uri.EscapeDataString(location)}&size=1";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", apiKey);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonResponse);

            var features = doc.RootElement.GetProperty("features");
            if (features.GetArrayLength() == 0)
                return null;

            var coords = features[0].GetProperty("geometry").GetProperty("coordinates");
            return (Latitude: coords[1].GetDouble(), Longitude: coords[0].GetDouble());
        }
    }
}
