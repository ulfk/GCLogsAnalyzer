
namespace GCLogsAnalyzer
{
    public class SimpleLogStat
    {
        public SimpleLogStat(string text, GeocacheLog log)
        {
            Text = text;
            Log = log;
        }

        public string Text { get; set; }

        public GeocacheLog Log { get; set; }
    }
}
