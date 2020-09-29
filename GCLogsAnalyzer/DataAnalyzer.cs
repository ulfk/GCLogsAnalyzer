using System;
using System.Collections.Generic;
using System.Linq;

namespace GCLogsAnalyzer
{
    public static class DataAnalyzer
    {
        private const string ColFoundIdx = "Found-Idx";
        private const string ColNr = "Nr.";
        private const string ColFound = "Found";
        private const string ColFounds = "Founds";
        private const string ColPlaced = "Placed";
        private const string ColGcCode = "GC-Code";
        private const string ColName = "Name";
        private const string ColType = "Type";
        private const string ColSize = "Size";
        private const string ColDifficulty = "Difficulty";
        private const string ColTerrain = "Terrain";
        private const string ColCountry = "Country";
        private const string ColPlacedBy = "Placed by";
        private const string ColCoords = "Coords";
        private const string ColLogType = "Log Type";
        private const string ColLog = "Log";
        private const string ColDescription = "Description";
        private const string TextVisitLog = "Visit Log";

        private static readonly TableColumn<GeocacheLog>[] FullInfoTableSpec =
        {
            new TableColumn<GeocacheLog>(ColNr,        (idx, log) => idx),
            new TableColumn<GeocacheLog>(ColFoundIdx,  (idx, log) => log.FoundIndex),
            new TableColumn<GeocacheLog>(ColFound,     (idx, log) => log.FoundDate),
            new TableColumn<GeocacheLog>(ColPlaced,    (idx, log) => log.Placed),
            new TableColumn<GeocacheLog>(ColGcCode,    (idx, log) => log.Code.ToLink(log.Code.ToCoordInfoUrl())),
            new TableColumn<GeocacheLog>(ColName,      (idx, log) => log.Name),
            new TableColumn<GeocacheLog>(ColType,      (idx, log) => log.Type),
            new TableColumn<GeocacheLog>(ColSize,      (idx, log) => log.Size),
            new TableColumn<GeocacheLog>(ColDifficulty,(idx, log) => log.Difficulty),
            new TableColumn<GeocacheLog>(ColTerrain,   (idx, log) => log.Terrain),
            new TableColumn<GeocacheLog>(ColCountry,   (idx, log) => log.Country),
            new TableColumn<GeocacheLog>(ColPlacedBy,  (idx, log) => log.PlacedBy.ToGcUserLink(log.OwnerId)),
            new TableColumn<GeocacheLog>(ColCoords,    (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<GeocacheLog>(ColLogType,   (idx, log) => log.LogType),
            new TableColumn<GeocacheLog>(ColLog,       (idx, log) => TextVisitLog.ToLogLink(log.LogId))
        };

        private static readonly TableColumn<GeocacheLog>[] ShortInfoTableSpec =
        {
            new TableColumn<GeocacheLog>(ColFoundIdx,  (idx, log) => log.FoundIndex),
            new TableColumn<GeocacheLog>(ColFound,     (idx, log) => log.FoundDate),
            new TableColumn<GeocacheLog>(ColPlaced,    (idx, log) => log.Placed),
            new TableColumn<GeocacheLog>(ColGcCode,    (idx, log) => log.Code.ToLink(log.Code.ToCoordInfoUrl())),
            new TableColumn<GeocacheLog>(ColName,      (idx, log) => log.Name),
            new TableColumn<GeocacheLog>(ColType,      (idx, log) => log.Type),
            new TableColumn<GeocacheLog>(ColSize,      (idx, log) => log.Size),
            new TableColumn<GeocacheLog>(ColDifficulty,(idx, log) => log.Difficulty),
            new TableColumn<GeocacheLog>(ColTerrain,   (idx, log) => log.Terrain),
            new TableColumn<GeocacheLog>(ColCountry,   (idx, log) => log.Country),
            new TableColumn<GeocacheLog>(ColPlacedBy,  (idx, log) => log.PlacedBy.ToGcUserLink(log.OwnerId)),
            new TableColumn<GeocacheLog>(ColCoords,    (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<GeocacheLog>(ColLogType,   (idx, log) => log.LogType),
            new TableColumn<GeocacheLog>(ColLog,       (idx, log) => TextVisitLog.ToLogLink(log.LogId))
        };

        private static readonly TableColumn<SimpleLogStat>[] SimpleLogStatTableSpec =
        {
            new TableColumn<SimpleLogStat>(ColDescription,(idx, stat) => stat.Text),
            new TableColumn<SimpleLogStat>(ColGcCode,     (idx, stat) => stat.Log.Code.ToLink(stat.Log.Code.ToCoordInfoUrl())),
            new TableColumn<SimpleLogStat>(ColFound,      (idx, stat) => stat.Log.FoundDate),
            new TableColumn<SimpleLogStat>(ColName,       (idx, stat) => stat.Log.Name),
            new TableColumn<SimpleLogStat>(ColType,       (idx, stat) => stat.Log.Type),
            new TableColumn<SimpleLogStat>(ColCoords,     (idx, stat) => stat.Log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn<SimpleLogStat>(ColLog,        (idx, stat) => TextVisitLog.ToLogLink(stat.Log.LogId))
        };

        private static TableColumn<SimpleStat>[] GetSimpleStatSpec(string text)
        {
            return new[]
            {
                new TableColumn<SimpleStat>(ColNr,     (idx, stat) => idx),
                new TableColumn<SimpleStat>(text,      (idx, stat) => stat.Text),
                new TableColumn<SimpleStat>(ColFounds, (idx, stat) => stat.Founds)
            };
        }

        /// <summary>
        /// List of analyzer methods to be executed.
        /// </summary>
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
