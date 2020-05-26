using System;
using System.Linq;

namespace GCLogsAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== GC Logs Analyzer ===");
            if (args.Length < 2)
            {
                Console.WriteLine("Expected Parameters: <GPX-Filename> <HTML-Output-Filename>");
                return;
            }

            var filename = args[0];
            var htmlFile = args[1];

            var converter = new GpxConverter(filename).Parse();
            
            //foreach (var log in converter.FoundLogs.OrderBy(l => l.FoundDate))
            //{
            //    Console.WriteLine($"{log.FoundDate:yyyy-MM-dd} | {log.Code} | {log.Type} | {log.Size} | {log.Country} | {log.Placed:yyyy-MM-dd} | {log.Name} | {log.PlacedBy}");
            //}
            Console.WriteLine($"{converter.FoundLogs.Count} logs read from file");

            var htmlGenerator = new HtmlGenerator();
            htmlGenerator.AddTableSection(converter.FoundLogs.OrderBy(f => f.FoundDate), "Logs by Found Date", "ByFoundDate");
            htmlGenerator.AddTableSection(converter.FoundLogs.OrderBy(f => f.Placed), "Logs by Placed Date", "ByPlacedDate");
            htmlGenerator.GenerateHtmlFile(htmlFile);
            Console.WriteLine($"HTML-Output written to '{htmlFile}'");
        }
    }
}
