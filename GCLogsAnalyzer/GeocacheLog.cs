using System;
using System.Globalization;

namespace GCLogsAnalyzer
{
    public class GeocacheLog
    {
        /// <summary>
        /// Geoache Placed Date
        /// </summary>
        public DateTime Placed { get; set; }

        /// <summary>
        /// GC-Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name of the cache
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL of the cache
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Coords of the cache
        /// </summary>
        public GeoLocation GeoLocation = new GeoLocation();

        /// <summary>
        /// Type of the cache
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Container size 
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Terrain rating
        /// </summary>
        public double Terrain { get; set; }

        /// <summary>
        /// Difficulty rating
        /// </summary>
        public double Difficulty { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Placed by (PlacedBy)
        /// </summary>
        public string PlacedBy { get; set; }

        /// <summary>
        /// Description of the cache
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Date when user found the cache
        /// </summary>
        public DateTime FoundDate { get; set; }

        /// <summary>
        /// Found log
        /// </summary>
        public string FoundLog { get; set; }

        /// <summary>
        /// Type of Log
        /// </summary>
        public string LogType { get; set; }
    }

    public class GeoLocation
    {
        public CultureInfo Culture = new CultureInfo("en-US");

        public double Lat { get; set; }

        public double Lon { get; set; }

        public string LatString => Lat.ToString(Culture);

        public string LonString => Lon.ToString(Culture);

        public override string ToString()
        {
            var lat = Math.Abs(Lat);
            var latChar = Lat >= 0.0 ? 'N' : 'S';
            var latInt = Math.Truncate(lat);
            var latDec = lat - latInt;

            var lon = Math.Abs(Lon);
            var lonChar = Lon >= 0.0 ? 'E' : 'W';
            var lonInt = Math.Truncate(lon);
            var lonDec = lon - lonInt;

            return $"{latChar} {latInt:00}° {latDec.ToString("00.000", Culture)} {lonChar} {lonInt:000}° {lonDec.ToString("00.000", Culture)}";
        }
    }
}
