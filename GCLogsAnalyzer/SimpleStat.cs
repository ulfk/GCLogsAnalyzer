
namespace GCLogsAnalyzer
{
    public class SimpleStat
    {
        public SimpleStat(string text, int founds)
        {
            Text = text;
            Founds = founds;
        }

        public string Text { get; set; }

        public int Founds { get; set; }
    }
}
