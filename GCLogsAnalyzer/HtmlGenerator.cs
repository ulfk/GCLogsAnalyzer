using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GCLogsAnalyzer;

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

    private readonly Dictionary<string, Section> _sections = new Dictionary<string, Section>();

    public void GenerateHtmlFile(string filename, bool addHtmlHeader = true)
    {
        var buffer = new StringBuilder();
        if(addHtmlHeader) 
            buffer.AppendLine(HtmlHelper.PageHeader);
        buffer.AppendLine(HtmlHelper.StyleAndSectionHeader);

        if (_sections.Count > 0)
        {
            buffer.AppendLine(HtmlHelper.Headline("main", "Geocaching Found Statistics", 1));
            // Table of Contents
            buffer.AppendLine(HtmlHelper.Headline("toc", "Contents"));
            buffer.AppendLine("<ul>");
            foreach (var (sectionLinkName, sectionData) in _sections)
            {
                buffer.AppendLine($"<li>{sectionData.Headline.ToLink($"#{sectionLinkName}", false)}</li>");
            }
            buffer.AppendLine("</ul>");

            // All previously generated sections
            foreach (var section in _sections)
            {
                buffer.AppendLine(section.Value.Content);
            }
        }

        buffer.AppendLine(HtmlHelper.StyleAndSectionFooter);
        if (addHtmlHeader)
            buffer.AppendLine(HtmlHelper.PageFooter);

        File.WriteAllText(filename, buffer.ToString());
    }

    public void AddTableSection<T>(IEnumerable<T> data, string headline, string name, TableColumn<T>[] tableSpec)
    {
        var buffer = new StringBuilder();
        buffer.AppendLine(HtmlHelper.Headline(name, headline));
        buffer.AppendLine(BackLink);
        var headerRow = HtmlHelper.TableRowHeader(tableSpec.Select(t => t.HeaderText).ToArray<object>());
        buffer.AppendLine(HtmlHelper.TableHeader(name, headerRow));
        var idx = 1;
        foreach (var item in data)
        {
            buffer.AppendLine(HtmlHelper.TableRow(tableSpec.Select(t => t.ValueFunc(idx, item)).ToArray()));
            idx++;
        }

        buffer.AppendLine(HtmlHelper.TableFooter);

        _sections.Add(name, new Section(headline, buffer.ToString()));
    }

    private static string BackLink => $"<div class=\"backlink\">{("&uarr; back".ToLink("#main", false))}</div>";
}