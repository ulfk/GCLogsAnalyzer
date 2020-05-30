using System;

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
            var htmlGenerator = new HtmlGenerator();

            var converter = new GpxConverter(filename).Parse();
            Console.WriteLine($"{converter.FoundLogs.Count} logs read from file");

            DataAnalyzer.Analyze(converter.FoundLogs, htmlGenerator);
            htmlGenerator.GenerateHtmlFile(htmlFile);
            Console.WriteLine($"HTML-Output written to '{htmlFile}'");
        }
    }
}
