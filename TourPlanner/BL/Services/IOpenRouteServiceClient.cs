using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Services
{
    public interface IOpenRouteServiceClient
    {
        Task<(double Distance, double Duration, string GeoJson, (double Latitude, double Longitude) FromCoords, (double Latitude, double Longitude) ToCoords)>
            GetRouteWithCoordinatesAsync(string from, string to);
    }
}
