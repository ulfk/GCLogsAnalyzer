using System;

namespace GCLogsAnalyzer
{
    public class GeoLocation
    {
        public double Lat { get; set; }

        public double Lon { get; set; }

        public string LatString => Lat.ToString(GroundspeakHelper.CultureInfo);

        public string LonString => Lon.ToString(GroundspeakHelper.CultureInfo);

        public override string ToString()
        {
            var (latChar, latDegree, latMin, latMinFraction) = GetPositionValues(Lat, 'N', 'S');
            var (lonChar, lonDegree, lonMin, lonMinFraction) = GetPositionValues(Lon, 'E', 'W');

            return $"{latChar} {latDegree:00}° {latMin:00}.{latMinFraction:000} {lonChar} {lonDegree:000}° {lonMin:00}.{lonMinFraction:000}";
        }

        private static (char, int, int, int) GetPositionValues(double value, char positive, char negative)
        {
            var charVal = value >= 0.0 ? positive : negative;
            var absValue = Math.Abs(value);
            var degree = Math.Truncate(absValue);
            var minutes = (absValue - degree) * 60.0;
            var minutesInt = Math.Truncate(minutes);
            var minutesFaction = Math.Round((minutes - minutesInt) * 1000.0);

            return (charVal, (int)degree, (int)minutesInt, (int)minutesFaction);
        }
    }
}