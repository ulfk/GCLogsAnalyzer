using System;
using System.Globalization;

namespace GCLogsAnalyzer
{
    public class GeoLocation
    {
        // currently we don't need to set this from outside so leave it private
        private readonly CultureInfo _culture = new CultureInfo("en-US");

        public double Lat { get; set; }

        public double Lon { get; set; }

        public string LatString => Lat.ToString(_culture);

        public string LonString => Lon.ToString(_culture);

        public override string ToString()
        {
            var latChar = Lat >= 0.0 ? 'N' : 'S';
            var lat = Math.Abs(Lat);
            var latInt = Math.Truncate(lat);
            var latDec = lat - latInt;

            var lonChar = Lon >= 0.0 ? 'E' : 'W';
            var lon = Math.Abs(Lon);
            var lonInt = Math.Truncate(lon);
            var lonDec = lon - lonInt;

            return $"{latChar} {latInt:00}° {latDec.ToString("00.000", _culture)} {lonChar} {lonInt:000}° {lonDec.ToString("00.000", _culture)}";
        }
    }
}