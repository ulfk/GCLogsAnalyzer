﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace GCLogsAnalyzer
{
    public class GpxConverter
    {
        private readonly string _filename;
        private readonly CultureInfo _cultureInfo;
        private readonly TimeZoneInfo _timeZone;

        public List<GeocacheLog> FoundLogs { get; set; }

        public GpxConverter(string filename)
        {
            _filename = filename;
            FoundLogs = new List<GeocacheLog>();
            _cultureInfo = GroundspeakHelper.CultureInfo;
            _timeZone = GroundspeakHelper.TimeZoneInfo;
        }

        public GpxConverter Parse()
        {
            using var fileStream = File.OpenRead(_filename);
            var xmlSettings = new XmlReaderSettings { CloseInput = true };
            using var reader = XmlReader.Create(fileStream, xmlSettings);
            
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "wpt")
                {
                    var log = ReadLogAndGeocacheDetails(reader);
                    FoundLogs.Add(log);
                }
            }

            var idx = 1;
            foreach (var log in FoundLogs.OrderBy(f => f.FoundDate))
                log.FoundIndex = idx++;

            return this;
        }


        private GeocacheLog ReadLogAndGeocacheDetails(XmlReader reader)
        {
            var log = new GeocacheLog();

            if (reader.HasAttributes)
            {
                log.GeoLocation.Lat = GetAttributeAsDouble(reader, "lat");
                log.GeoLocation.Lon = GetAttributeAsDouble(reader, "lon");
            }

            while (reader.Read())
            {
                if (reader.Name == "wpt")
                    break;
                if (reader.NodeType != XmlNodeType.Element)
                    continue;
                switch (reader.Name)
                {
                    case "groundspeak:logs": ReadLogEntrySection(reader, log); break;
                    case "time": log.Placed = GetElementAsDateTime(reader); break;
                    case "name": log.Code = GetElementAsString(reader); break;
                    case "url": log.Url = GetElementAsString(reader); break;
                    case "groundspeak:name": log.Name = GetElementAsString(reader); break;
                    case "groundspeak:long_description": log.Description = GetElementAsString(reader); break;
                    case "groundspeak:type": log.Type = GetElementAsString(reader); break;
                    case "groundspeak:container": log.Size = GetElementAsString(reader); break;
                    case "groundspeak:country": log.Country = GetElementAsString(reader); break;
                    case "groundspeak:state": log.State = GetElementAsString(reader); break;
                    case "groundspeak:owner":
                        if (reader.HasAttributes) log.OwnerId = GetAttributeAsString(reader, "id");
                        log.PlacedBy = GetElementAsString(reader); 
                        break;
                    case "groundspeak:difficulty": log.Difficulty = GetElementAsDouble(reader); break;
                    case "groundspeak:terrain": log.Terrain = GetElementAsDouble(reader); break;
                }
            }

            return log;
        }

        private void ReadLogEntrySection(XmlReader reader, GeocacheLog log)
        {
            while (reader.Read())
            {
                if (reader.Name == "groundspeak:logs")
                    break;
                if (reader.NodeType != XmlNodeType.Element || reader.Name != "groundspeak:log") 
                    continue;

                if (reader.HasAttributes)
                {
                    log.LogId = GetAttributeAsString(reader, "id");
                }

                var foundDate = DateTime.MinValue;
                var foundLogMsg = "";
                var logType = "";
                while (reader.Read())
                {
                    if (reader.Name == "groundspeak:log")
                        break;
                    if (reader.NodeType != XmlNodeType.Element)
                        continue;
                    switch (reader.Name)
                    {
                        case "groundspeak:date": foundDate = GetElementAsDateTime(reader); break;
                        case "groundspeak:text": foundLogMsg = GetElementAsString(reader); break;
                        case "groundspeak:type": logType = GetElementAsString(reader); break;
                    }
                }

                if (logType.IsValidLogType())
                {
                    log.FoundDate = foundDate;
                    log.FoundLog = foundLogMsg;
                    log.LogType = logType;
                }
            }
        }

        private double GetAttributeAsDouble(XmlReader reader, string name) => ToDouble(reader.GetAttribute(name));

        private static string GetAttributeAsString(XmlReader reader, string name) => reader.GetAttribute(name);

        private static string GetElementAsString(XmlReader reader) => reader.ReadElementContentAsString();

        private DateTime GetElementAsDateTime(XmlReader reader)
        {
            var value = GetElementAsString(reader);
            var date = DateTime.Parse(value);
            return TimeZoneInfo.ConvertTime(date, _timeZone);
        }

        private double GetElementAsDouble(XmlReader reader)
        {
            var value = GetElementAsString(reader);
            return ToDouble(value);
        }

        private double ToDouble(string value)
        {
            return double.Parse(value, _cultureInfo);
        }
    }
}