using System;

namespace GCLogsAnalyzer;

public class GeocacheLog
{
    public int FoundIndex { get; set; }

    /// <summary>
    /// Geoache Placed Date
    /// </summary>
    public DateTime Placed { get; set; }

    /// <summary>
    /// GC-Code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Name of the cache
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL of the cache
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Coords of the cache
    /// </summary>
    public GeoLocation GeoLocation = new GeoLocation();

    /// <summary>
    /// Type of the cache
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Container size 
    /// </summary>
    public string Size { get; set; } = string.Empty;

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
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// State
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Placed by (PlacedBy)
    /// </summary>
    public string PlacedBy { get; set; } = string.Empty;

    /// <summary>
    /// Id of Cache Owner
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Description of the cache
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ID of the log entry
    /// </summary>
    public string LogId { get; set; } = string.Empty;

    /// <summary>
    /// Date when user found the cache
    /// </summary>
    public DateTime FoundDate { get; set; }

    /// <summary>
    /// Found log
    /// </summary>
    public string FoundLog { get; set; } = string.Empty;

    /// <summary>
    /// Type of Log
    /// </summary>
    public string LogType { get; set; } = string.Empty;

    /// <summary>
    /// Is cache archived?
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Is cache available?
    /// </summary>
    public bool Available { get; set; }
}
