using System;
using System.IO;

namespace GCLogsAnalyzer;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== GC Logs Analyzer ===");
        if (args.Length < 2)
        {
            Console.WriteLine("Expected Parameters: <GPX-Filename> <HTML-Output-Filename>");
            return;
        }

        var filename = args[0];
        var htmlFile = args[1];
        var htmlGenerator = new HtmlGenerator();

        Console.WriteLine($"Reading logs from '{filename}'");
        using var fileStream = File.OpenRead(filename);
        var converter = new GpxConverter().Parse(fileStream);
        Console.WriteLine($"{converter.FoundLogs.Count} logs read from file");

        DataAnalyzer.Analyze(converter.FoundLogs, htmlGenerator, Console.WriteLine);
        htmlGenerator.GenerateHtmlFile(htmlFile);
        Console.WriteLine($"HTML-Output written to '{htmlFile}'");

        //var dynamicHtmlFile = $"{htmlFile}-js.html";
        //htmlGenerator.GenerateDynamicHtmlFile(dynamicHtmlFile, converter.FoundLogs);
        //Console.WriteLine($"Dynamic HTML-Output written to '{dynamicHtmlFile}'");
    }
}
