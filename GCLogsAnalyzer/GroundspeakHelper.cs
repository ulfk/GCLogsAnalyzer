﻿using System;
using System.Globalization;
using System.Linq;

namespace GCLogsAnalyzer
{
    public static class GroundspeakHelper
    {
        // Bookmark: https://www.geocachingtoolbox.com/

        // constants to calculate the GC/GL-codes from an ID
        private const string LookupTable = "0123456789ABCDEFGHJKMNPQRTVWXYZ";
        private const int BaseValue = 31;
        private const int IdOffset = 411120;

        /// <summary>
        /// Calculation found here: http://kryptografie.de/kryptografie/chiffre/gc-code.htm
        /// Test cases:
        ///   "Petit":                    30236633 -> "125QG6"
        ///   "Goeteborg Magnetic":       30142402 -> "122JEF"
        ///   "Am Brombeerweg":          303712500 -> "AK9HRB"
        ///   "Volksdorfer Wochenmarkt": 859171469 -> "Z0QQRK"
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static string CalcCodeFromId(int logId, string prefix = "GL")
        {
            var result = "";
            var id = logId + IdOffset;
            while (id > 0)
            {
                var rest = id % BaseValue;
                id = (id - rest) / BaseValue;
                result = LookupTable[rest] + result;
            }

            return prefix + result;
        }

        private static int GetIdFromCode(string code, string prefixToRemove = null)
        {
            var factor = 1;
            var result = 0;
            var value = code;
            if (!string.IsNullOrEmpty(prefixToRemove) && code.StartsWith(prefixToRemove))
                value = code.Substring(prefixToRemove.Length);
            
            for (var idx = value.Length - 1; idx >= 0; idx--)
            {
                result += LookupTable.IndexOf(value[idx]) * factor;
                factor *= BaseValue;
            }

            return result - IdOffset;
        }

        public static string GetLogUrl(string logId)
        {
            var logIdNumeric = int.Parse(logId);
            var logCode = CalcCodeFromId(logIdNumeric);

            return $"https://coord.info/{logCode}";
        }

        public static string GetUserUrl(string user) => $"https://www.geocaching.com/p/default.aspx?id={user}";

        // used to convert float numbers that are currently stored in en-US style in the GPX
        public static CultureInfo CultureInfo => new CultureInfo("en-US");

        // all timestamps in the GPX are currently in Pacific Standard Time
        public static TimeZoneInfo TimeZoneInfo => TimeZoneInfo.GetSystemTimeZones().First(tz => tz.Id == "Pacific Standard Time");

        public static bool IsValidLogType(string logType) => logType == "Found it" || logType == "Webcam Photo Taken" || logType == "Attended";
    }
}