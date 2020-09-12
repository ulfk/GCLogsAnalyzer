using System;
using System.Collections.Generic;
using System.Linq;

namespace GCLogsAnalyzer
{
    public static class DataAnalyzer
    {
        private static readonly TableColumn<GeocacheLog>[] FullInfoTableSpec =
        {
            new TableColumn<GeocacheLog>("Nr.",        (idx, log) => idx),
            new TableColumn<GeocacheLog>("Found-Idx",  (idx, log) => log.FoundIndex),
            new TableColumn<GeocacheLog>("Found",      (idx, log) => log.FoundDate),
            new TableColumn<GeocacheLog>("Placed",     (idx, log) => log.Placed),
            new TableColumn<GeocacheLog>("GC-Code",    (idx, log) => log.Code.ToLink(log.Code.ToCoordInfoUrl())),
            new TableColumn<GeocacheLog>("Name",       (idx, log) => log.Name),
            new TableColumn<GeocacheLog>("Type",       (idx, log) => log.Type),
            new TableColumn<GeocacheLog>("Size",       (idx, log) => log.Size),
            new TableColumn<GeocacheLog>("Difficulty", (idx, log) => log.Difficulty),
            new TableColumn<GeocacheLog>("Terrain",    (idx, log) => log.Terrain),
            new TableColumn<GeocacheLog>("Country",    (idx, log) => log.Country),
            new TableColumn<GeocacheLog>("PlacedBy",   (idx, log) => log.PlacedBy.ToGcUserLink(log.OwnerId)),
            new TableColumn<GeocacheLog>("Coords",     (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<GeocacheLog>("LogType",    (idx, log) => log.LogType),
            new TableColumn<GeocacheLog>("Log",        (idx, log) => "Visit Log".ToLogLink(log.LogId))
        };

        private static readonly TableColumn<GeocacheLog>[] ShortInfoTableSpec =
        {
            new TableColumn<GeocacheLog>("Found-Idx",  (idx, log) => log.FoundIndex),
            new TableColumn<GeocacheLog>("Found",      (idx, log) => log.FoundDate),
            new TableColumn<GeocacheLog>("Placed",     (idx, log) => log.Placed),
            new TableColumn<GeocacheLog>("GC-Code",    (idx, log) => log.Code.ToLink(log.Code.ToCoordInfoUrl())),
            new TableColumn<GeocacheLog>("Name",       (idx, log) => log.Name),
            new TableColumn<GeocacheLog>("Type",       (idx, log) => log.Type),
            new TableColumn<GeocacheLog>("Size",       (idx, log) => log.Size),
            new TableColumn<GeocacheLog>("Difficulty", (idx, log) => log.Difficulty),
            new TableColumn<GeocacheLog>("Terrain",    (idx, log) => log.Terrain),
            new TableColumn<GeocacheLog>("Country",    (idx, log) => log.Country),
            new TableColumn<GeocacheLog>("PlacedBy",   (idx, log) => log.PlacedBy.ToGcUserLink(log.OwnerId)),
            new TableColumn<GeocacheLog>("Coords",     (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<GeocacheLog>("LogType",    (idx, log) => log.LogType),
            new TableColumn<GeocacheLog>("Log",        (idx, log) => "Visit Log".ToLogLink(log.LogId))
        };

        private static readonly TableColumn<SimpleLogStat>[] SimpleLogStatTableSpec =
        {
            new TableColumn<SimpleLogStat>("Description", (idx, stat) => stat.Text),
            new TableColumn<SimpleLogStat>("GC-Code",     (idx, stat) => stat.Log.Code.ToLink(stat.Log.Code.ToCoordInfoUrl())),
            new TableColumn<SimpleLogStat>("Found",       (idx, stat) => stat.Log.FoundDate),
            new TableColumn<SimpleLogStat>("Name",        (idx, stat) => stat.Log.Name),
            new TableColumn<SimpleLogStat>("Type",        (idx, stat) => stat.Log.Type),
            new TableColumn<SimpleLogStat>("Coords",      (idx, stat) => stat.Log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<SimpleLogStat>("Log",         (idx, stat) => "Visit Log".ToLogLink(stat.Log.LogId))
        };

        private static TableColumn<SimpleStat>[] GetSimpleStatSpec(string text)
        {
            return new[]
            {
                new TableColumn<SimpleStat>("Nr",     (idx, stat) => idx),
                new TableColumn<SimpleStat>(text,     (idx, stat) => stat.Text),
                new TableColumn<SimpleStat>("Founds", (idx, stat) => stat.Founds)
            };
        }

        private static readonly Action<IEnumerable<GeocacheLog>, HtmlGenerator>[] AnalyzingMethods = 
        {
            FoundsByCountry,
            FoundsByState,
            FoundsByCacheType,
            FoundsByContainerSize,
            AnniversaryFounds,
            FoundsByOwner,
            FarthestAwayFoundsByCardinalDirection,
            FoundsByFoundDate,
            FoundsByPlacedDate
        };

        public static void Analyze(List<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            foreach (var analyzingMethod in AnalyzingMethods)
            {
                analyzingMethod(foundLogs, htmlGenerator);
            }
        }

        private static void FarthestAwayFoundsByCardinalDirection(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            // Farthest away founds by cardinal direction
            GeocacheLog maxNorth = null;
            GeocacheLog maxSouth = null;
            GeocacheLog maxEast = null;
            GeocacheLog maxWest = null;
            foreach (var geocacheLog in foundLogs)
            {
                if (maxNorth == null || maxNorth.GeoLocation.Lat < geocacheLog.GeoLocation.Lat)
                    maxNorth = geocacheLog;
                if (maxSouth == null || maxSouth.GeoLocation.Lat > geocacheLog.GeoLocation.Lat)
                    maxSouth = geocacheLog;
                if (maxEast == null || maxEast.GeoLocation.Lon < geocacheLog.GeoLocation.Lon)
                    maxEast = geocacheLog;
                if (maxWest == null || maxWest.GeoLocation.Lon > geocacheLog.GeoLocation.Lon)
                    maxWest = geocacheLog;
            }

            var stats = new List<SimpleLogStat>
            {
                new SimpleLogStat("Farthest North", maxNorth),
                new SimpleLogStat("Farthest South", maxSouth),
                new SimpleLogStat("Farthest East", maxEast),
                new SimpleLogStat("Farthest West", maxWest)
            };

            htmlGenerator.AddTableSection(stats, "Cardinal Direction Maximums", "CardinalDirectionMaximums", SimpleLogStatTableSpec);
        }

        private static void FoundsByPlacedDate(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            htmlGenerator.AddTableSection(
                foundLogs.OrderBy(f => f.Placed), 
                "Logs by Placed Date", 
                "ByPlacedDate",
                FullInfoTableSpec);
        }

        private static void FoundsByFoundDate(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            htmlGenerator.AddTableSection(
                foundLogs.OrderBy(f => f.FoundDate), 
                "Logs by Found Date", 
                "ByFoundDate",
                ShortInfoTableSpec);
        }

        private static void FoundsByOwner(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            var ownerStats = foundLogs
                .GroupBy(l => l.PlacedBy)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .Where(x => x.Founds >= 5)
                .OrderByDescending(s => s.Founds);

            htmlGenerator.AddTableSection(
                ownerStats, 
                "Founds by Owner (five and more founds)", 
                "FoundsByOwner",
                GetSimpleStatSpec("Owner"));
        }

        private static void AnniversaryFounds(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            var anniversaryList = foundLogs
                .OrderBy(f => f.FoundDate)
                .Where((l, i) => i == 0 || (i + 1) % 100 == 0);
            htmlGenerator.AddTableSection(anniversaryList, "Every 100th Found", "Every100thFound", ShortInfoTableSpec);
        }

        private static void FoundsByContainerSize(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            var sizeStats = foundLogs
                .GroupBy(l => l.Size)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);

            htmlGenerator.AddTableSection(sizeStats, "Founds by Container Size", "FoundsByContainerSize",
                GetSimpleStatSpec("Size"));
        }

        private static void FoundsByCacheType(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            var typeStats = foundLogs
                .GroupBy(l => l.Type)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);

            htmlGenerator.AddTableSection(
                typeStats, 
                "Founds by Cache Type", 
                "FoundsByCacheType",
                GetSimpleStatSpec("Cache Type"));
        }

        private static void FoundsByState(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            var stateStats = foundLogs
                .Where(l => l.Country == "Germany")
                .GroupBy(l => l.State)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);

            htmlGenerator.AddTableSection(
                stateStats, 
                "Founds by 'Bundesland'", 
                "FoundsByBundesland",
                GetSimpleStatSpec("Bundesland"));
        }

        private static void FoundsByCountry(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
        {
            var countryStats = foundLogs
                .GroupBy(l => l.Country)
                .Select(x => new SimpleStat(x.Key, x.Count()))
                .OrderByDescending(s => s.Founds);

            htmlGenerator.AddTableSection(countryStats, "Founds by Country", "FoundsByCountry", GetSimpleStatSpec("Country"));
        }
    }
}
