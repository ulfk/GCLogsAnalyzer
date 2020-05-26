using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace GCLogsAnalyzer
{
    public class HtmlGenerator
    {
        private class Section
        {
            public Section(string headline, string content)
            {
                Headline = headline;
                Content = content;
            }

            public string Headline { get; }
            public string Content { get; }
        }
        private Dictionary<string, Section> _sections = new Dictionary<string, Section>();

        class TableColumn
        {
            public TableColumn(string headerText, Func<int, GeocacheLog, object> valueFunc)
            {
                HeaderText = headerText;
                ValueFunc = valueFunc;
            }

            public string HeaderText { get; }
            public Func<int,GeocacheLog,object> ValueFunc { get; }
        }

        private readonly TableColumn[] _fullInfoTableSpec = 
        {
            new TableColumn ("Nr.",        (idx, log) => idx),
            new TableColumn ("Found",      (idx, log) => log.FoundDate),
            new TableColumn ("GC-Code",    (idx, log) => log.Code.ToLink(log.Url)),
            new TableColumn ("Name",       (idx, log) => log.Name),
            new TableColumn ("Type",       (idx, log) => log.Type),
            new TableColumn ("Size",       (idx, log) => log.Size),
            new TableColumn ("Difficulty", (idx, log) => log.Difficulty),
            new TableColumn ("Terrain",    (idx, log) => log.Terrain),
            new TableColumn ("Country",    (idx, log) => log.Country),
            new TableColumn ("Placed",     (idx, log) => log.Placed),
            new TableColumn ("PlacedBy",   (idx, log) => log.PlacedBy),
            new TableColumn ("Coords",     (idx, log) => log.GeoLocation.ToGoogleMapsLink()),
            new TableColumn ("LogType",    (idx, log) => log.LogType)
        };

        public void GenerateHtmlFile(string filename)
        {
            var buffer = new StringBuilder();
            buffer.AppendLine(HtmlHelper.PageHeader);

            if (_sections.Count > 0)
            {
                buffer.AppendLine(HtmlHelper.Headline("toc", "Contents"));
                buffer.AppendLine("<ul>");
                foreach (var section in _sections)
                {
                    buffer.AppendLine($"<li>{section.Value.Headline.ToLink($"#{section.Key}", false)}</li>");
                }
                buffer.AppendLine("</ul>");

                foreach (var section in _sections)
                {
                    buffer.AppendLine(section.Value.Content);
                }
            }

            buffer.AppendLine(HtmlHelper.PageFooter);

            File.WriteAllText(filename, buffer.ToString());
        }

        public void AddTableSection(IEnumerable<GeocacheLog> founds, string headline, string name)
        {
            
            var buffer = new StringBuilder();
            buffer.AppendLine(HtmlHelper.Headline(name, headline));
            var headerRow = HtmlHelper.TableRowHeader(_fullInfoTableSpec.Select(t => t.HeaderText).ToArray());
            buffer.AppendLine(HtmlHelper.TableHeader(name, headerRow));
            var idx = 1;
            foreach (var found in founds)
            {
                buffer.AppendLine(HtmlHelper.TableRow(_fullInfoTableSpec.Select(t => t.ValueFunc(idx, found)).ToArray()));
                idx++;
            }

            buffer.AppendLine(HtmlHelper.TableFooter);

            _sections.Add(name, new Section(headline, buffer.ToString()));
        }

        //public string GetSection(string name)
        //{
        //    return _sections.TryGetValue(name, out var value) ? value : string.Empty;
        //}
    }
}