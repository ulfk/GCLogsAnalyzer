using System;
using System.Collections.Generic;
using System.Linq;

namespace GCLogsAnalyzer;

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

    private static TableColumn<GeocacheLog> CreateLogColumn(
        string headerText,
        Func<int, GeocacheLog, object> valueFunc)
        => new TableColumn<GeocacheLog>(headerText, valueFunc);

    private static TableColumn<SimpleLogStat> CreateLogStatColumn(
        string headerText,
        Func<int, SimpleLogStat, object> valueFunc)
        => new TableColumn<SimpleLogStat>(headerText, valueFunc);

    private static TableColumn<SimpleStat> CreateStatColumn(
        string headerText,
        Func<int, SimpleStat, object> valueFunc)
        => new TableColumn<SimpleStat>(headerText, valueFunc);

    private static readonly TableColumn<GeocacheLog>[] FullInfoTableSpec =
    {
        CreateLogColumn(ColNr,        (idx, log) => idx),
        CreateLogColumn(ColFoundIdx,  (idx, log) => log.FoundIndex),
        CreateLogColumn(ColFound,     (idx, log) => log.FoundDate),
        CreateLogColumn(ColPlaced,    (idx, log) => log.Placed),
        CreateLogColumn(ColGcCode,    (idx, log) => log.ToCodeLinkWithState()),
        CreateLogColumn(ColName,      (idx, log) => log.Name),
        CreateLogColumn(ColType,      (idx, log) => log.Type),
        CreateLogColumn(ColSize,      (idx, log) => log.Size),
        CreateLogColumn(ColDifficulty,(idx, log) => log.Difficulty),
        CreateLogColumn(ColTerrain,   (idx, log) => log.Terrain),
        CreateLogColumn(ColCountry,   (idx, log) => log.Country),
        CreateLogColumn(ColPlacedBy,  (idx, log) => log.PlacedBy.ToGcUserLink(log.OwnerId)),
        CreateLogColumn(ColCoords,    (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
        CreateLogColumn(ColLogType,   (idx, log) => log.LogType),
        CreateLogColumn(ColLog,       (idx, log) => TextVisitLog.ToLogLink(log.LogId))
    };

    private static readonly TableColumn<GeocacheLog>[] ShortInfoTableSpec =
    {
        CreateLogColumn(ColFoundIdx,  (idx, log) => log.FoundIndex),
        CreateLogColumn(ColFound,     (idx, log) => log.FoundDate),
        CreateLogColumn(ColPlaced,    (idx, log) => log.Placed),
        CreateLogColumn(ColGcCode,    (idx, log) => log.ToCodeLinkWithState()),
        CreateLogColumn(ColName,      (idx, log) => log.Name),
        CreateLogColumn(ColType,      (idx, log) => log.Type),
        CreateLogColumn(ColSize,      (idx, log) => log.Size),
        CreateLogColumn(ColDifficulty,(idx, log) => log.Difficulty),
        CreateLogColumn(ColTerrain,   (idx, log) => log.Terrain),
        CreateLogColumn(ColCountry,   (idx, log) => log.Country),
        CreateLogColumn(ColPlacedBy,  (idx, log) => log.PlacedBy.ToGcUserLink(log.OwnerId)),
        CreateLogColumn(ColCoords,    (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
        CreateLogColumn(ColLogType,   (idx, log) => log.LogType),
        CreateLogColumn(ColLog,       (idx, log) => TextVisitLog.ToLogLink(log.LogId))
    };

    private static readonly TableColumn<SimpleLogStat>[] SimpleLogStatTableSpec =
    {
        CreateLogStatColumn(ColDescription,(idx, stat) => stat.Text),
        CreateLogStatColumn(ColGcCode,     (idx, stat) => stat.Log.ToCodeLinkWithState()),
        CreateLogStatColumn(ColFound,      (idx, stat) => stat.Log.FoundDate),
        CreateLogStatColumn(ColName,       (idx, stat) => stat.Log.Name),
        CreateLogStatColumn(ColType,       (idx, stat) => stat.Log.Type),
        CreateLogStatColumn(ColCoords,     (idx, stat) => stat.Log.GeoLocation.ToGoogleMapsLink()),
        CreateLogStatColumn(ColLog,        (idx, stat) => TextVisitLog.ToLogLink(stat.Log.LogId))
    };

    private static TableColumn<SimpleStat>[] GetSimpleStatSpec(string text)
    {
        return new[]
        {
            CreateStatColumn(ColNr,     (idx, stat) => idx),
            CreateStatColumn(text,      (idx, stat) => stat.Text),
            CreateStatColumn(ColFounds, (idx, stat) => stat.Founds)
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

    public static void Analyze(List<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator, Action<string> logMethod)
    {
        if(htmlGenerator == null) throw new ArgumentNullException(nameof(htmlGenerator));
        if(foundLogs == null) throw new ArgumentNullException(nameof(foundLogs));

        foreach (var analyzingMethod in AnalyzingMethods)
        {
            logMethod?.Invoke($"Adding {analyzingMethod.Method.Name}");
            analyzingMethod(foundLogs, htmlGenerator);
        }
    }

    private static void FarthestAwayFoundsByCardinalDirection(IEnumerable<GeocacheLog> foundLogs, HtmlGenerator htmlGenerator)
    {
        if (!foundLogs.Any()) return;

        // Farthest away founds by cardinal direction
        GeocacheLog? maxNorth = null;
        GeocacheLog? maxSouth = null;
        GeocacheLog? maxEast = null;
        GeocacheLog? maxWest = null;
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
            new SimpleLogStat("Farthest North", maxNorth!),
            new SimpleLogStat("Farthest South", maxSouth!),
            new SimpleLogStat("Farthest East", maxEast!),
            new SimpleLogStat("Farthest West", maxWest!)
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
