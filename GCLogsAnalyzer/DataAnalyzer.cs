using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCLogsAnalyzer
{
    public static class DataAnalyzer
    {
        private static readonly TableColumn<GeocacheLog>[] _fullInfoTableSpec =
{
            new TableColumn<GeocacheLog> ("Nr.",        (idx, log) => idx),
            new TableColumn<GeocacheLog> ("Found-Idx",  (idx, log) => log.FoundIndex),
            new TableColumn<GeocacheLog> ("Found",      (idx, log) => log.FoundDate),
            new TableColumn<GeocacheLog> ("GC-Code",    (idx, log) => log.Code.ToLink(log.Url)),
            new TableColumn<GeocacheLog> ("Name",       (idx, log) => log.Name),
            new TableColumn<GeocacheLog> ("Type",       (idx, log) => log.Type),
            new TableColumn<GeocacheLog> ("Size",       (idx, log) => log.Size),
            new TableColumn<GeocacheLog> ("Difficulty", (idx, log) => log.Difficulty),
            new TableColumn<GeocacheLog> ("Terrain",    (idx, log) => log.Terrain),
            new TableColumn<GeocacheLog> ("Country",    (idx, log) => log.Country),
            new TableColumn<GeocacheLog> ("Placed",     (idx, log) => log.Placed),
            new TableColumn<GeocacheLog> ("PlacedBy",   (idx, log) => log.PlacedBy),
            new TableColumn<GeocacheLog> ("Coords",     (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<GeocacheLog> ("LogType",    (idx, log) => log.LogType)
        };

        private static readonly TableColumn<GeocacheLog>[] _shortInfoTableSpec =
        {
            new TableColumn<GeocacheLog> ("Found-Idx",  (idx, log) => log.FoundIndex),
            new TableColumn<GeocacheLog> ("Found",      (idx, log) => log.FoundDate),
            new TableColumn<GeocacheLog> ("GC-Code",    (idx, log) => log.Code.ToLink(log.Url)),
            new TableColumn<GeocacheLog> ("Name",       (idx, log) => log.Name),
            new TableColumn<GeocacheLog> ("Type",       (idx, log) => log.Type),
            new TableColumn<GeocacheLog> ("Size",       (idx, log) => log.Size),
            new TableColumn<GeocacheLog> ("Difficulty", (idx, log) => log.Difficulty),
            new TableColumn<GeocacheLog> ("Terrain",    (idx, log) => log.Terrain),
            new TableColumn<GeocacheLog> ("Country",    (idx, log) => log.Country),
            new TableColumn<GeocacheLog> ("Placed",     (idx, log) => log.Placed),
            new TableColumn<GeocacheLog> ("PlacedBy",   (idx, log) => log.PlacedBy),
            new TableColumn<GeocacheLog> ("Coords",     (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<GeocacheLog> ("LogType",    (idx, log) => log.LogType)
        };

        private static TableColumn<SimpleStat>[] GetSimpleStatSpec(string text)
        {
            return new TableColumn<SimpleStat>[]
            {
                    new TableColumn<SimpleStat>(text, (idx, stat) => stat.Text),
                new TableColumn<SimpleStat>("Founds", (idx, stat) => stat.Founds)
            };
        }

        public static void Analyze(List<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            // Founds by Country
            var countryStats = foundLogs
                .GroupBy(l => l.Country)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);
            htmlGenerator.AddTableSection(countryStats, "Founds by Country", "FoundsByCountry", GetSimpleStatSpec("Country"));

            // Founds by Owner
            var ownerStats = foundLogs
                .GroupBy(l => l.PlacedBy)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .Where(x => x.Founds >= 5)
                .OrderByDescending(s => s.Founds);
            htmlGenerator.AddTableSection(ownerStats, "Founds by Owner (five and more founds)", "FoundsByOwner", GetSimpleStatSpec("Owner"));

            // Founds by Cache Type
            var typeStats = foundLogs
                .GroupBy(l => l.Type)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);
            htmlGenerator.AddTableSection(typeStats, "Founds by Cache Type", "FoundsByCacheType", GetSimpleStatSpec("Cache Type"));

            // Founds by Container Size
            var sizeStats = foundLogs
                .GroupBy(l => l.Size)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);
            htmlGenerator.AddTableSection(sizeStats, "Founds by Container Size", "FoundsByContainerSize", GetSimpleStatSpec("Size"));

            // Anniversary Founds
            var anniversaryList = foundLogs
                .OrderBy(f => f.FoundDate)
                .Where((l, i) => i == 0 || (i + 1) % 100 == 0);
            htmlGenerator.AddTableSection(anniversaryList, "Every 100th Found", "Every100thFound", _shortInfoTableSpec);
            
            // Founds by Found Date
            htmlGenerator.AddTableSection(foundLogs.OrderBy(f => f.FoundDate), "Logs by Found Date", "ByFoundDate", _shortInfoTableSpec);
            
            // Found by Placed Date
            htmlGenerator.AddTableSection(foundLogs.OrderBy(f => f.Placed), "Logs by Placed Date", "ByPlacedDate", _fullInfoTableSpec);
        }
    }
}
