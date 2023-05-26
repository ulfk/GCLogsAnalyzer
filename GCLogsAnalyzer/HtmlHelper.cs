using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCLogsAnalyzer;

public static class HtmlHelper
{
    public static string TableRow(params object[] values) => Row(values, false);

    public static string TableRowHeader(params object[] values) => Row(values, true);

    private static string Row(IEnumerable<object> values, bool isHeader)
    {
        var buffer = new StringBuilder();
        buffer.Append("<tr>");
        var cellTag = isHeader ? "th" : "td";
        foreach (var value in values)
        {
            buffer.Append($"<{cellTag}>{ValueToString(value)}</{cellTag}>");
        }

        buffer.Append("</tr>");
        return buffer.ToString();
    }

    public static string Headline(string name, string text, int level = 2) => $"<h{level} id=\"{name}\">{text}</h{level}>";

    /* Fixed Table-Header:
     https://css-tricks.com/position-sticky-and-table-headers/
     https://stackoverflow.com/questions/19559197/how-to-make-scrollable-table-with-fixed-headers-using-css
    */

    public static string PageHeaderBase(string additionalHeadTags = "") => @"<!DOCTYPE html>
<html>
<head>
<title>My Geocache Founds</title>
"+additionalHeadTags+@"
</head>
<body>";

    public static string PageHeader => PageHeaderBase();
    public static string PageHeaderWithJquery => PageHeaderBase(JqueryHeader);

    public const string JqueryHeader = "<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js\"></script>";

    public const string PageFooter = "</body></html>";

    public const string StyleAndSectionHeader = @"
<div id=""gc-logs-analyzed"">
<style type=""text/css"" scoped>
#gc-logs-analyzed {
    font-family: ""Arial"";
    padding: 10px;
}
table {
    border-spacing: 0px;
    border-collapse: collapse; 
    position: relative;
}
th, td {
    padding: 2px;
}
th {
    background-color: #f0f0f0;
}
table, td, th {
        border: 1px solid lightgray;
}
th {
  position: sticky;
  top: 0;
  box-shadow: 0 2px 2px -1px rgba(0, 0, 0, 0.4);
}
div.backlink {
  font-size: 80%;
  margin-bottom: 10px;
}
div.table {
   display: inline-block;
    overflow-y: auto;
    max-height:500px;
    border: 1px solid #ddd;
}

/* Tooltip container (copied from: https://www.w3schools.com/css/css_tooltip.asp) */
.tooltip {
  /*position: relative;*/
  display: inline-block;
  border-bottom: 1px dotted black;
}
/* Tooltip text */
.tooltip .tooltiptext {
  visibility: hidden;
  
  background-color: #eee;
  color: #333;
  text-align: left;
  padding: 6px 6px;
  border-radius: 6px;
 
  position: absolute;
  z-index: 1;
}
/* Show the tooltip text when you mouse over the tooltip container */
.tooltip:hover .tooltiptext {
  visibility: visible;
}
</style>
";

    public static string ToTextWithTooltip(this string text, IEnumerable<string> tooltipLines)
        => $"<div class=\"tooltip\">{text}<span class=\"tooltiptext\">{string.Join("<br>", tooltipLines.Select(x => "<nobr>"+x+"</nobr>"))}</span></div>";

    public static string GetAttributes(this GeocacheLog log)
    {
        var attrCount = log.Attributes.Count;
        if (attrCount == 0) return "&nbsp; - &nbsp;";
        var text = $"{attrCount} Attribute{(attrCount > 1 ? "s" : "")}";
        return text.ToTextWithTooltip(log.Attributes.Select(x => (x.Inverted ? "Not: " : "") + x.Name));
    }

    public const string StyleAndSectionFooter = "</div>";

    public static string TableHeader(string sectionName, string headerRow) 
        => $"<div id=\"{sectionName}\" class=\"table\">\n<table><thead>{headerRow}</thead>\n<tbody>";

    public const string TableFooter = "</tbody>\n</table>\n</div>";
        
    public static string ToLink(this string text, string url, bool openInBlank = true)
        => $"<a href=\"{url}\" {(openInBlank ? "target=\"_blank\"" : "")}>{text}</a>";

    public static string ToCodeLinkWithState(this GeocacheLog log)
        => log.GetStateIcon() + "&nbsp;" + log.Code.ToLink(log.Code.ToCoordInfoUrl());

    private static string GetStateIcon(this  GeocacheLog log)
        => log.Archived ? "&#x2612;" : (log.Available ? "&#x2611" : "&#x2610");

    public static string ToGcUserLink(this string text, string userId)
        => text.ToLink(userId.GetUserUrl());

    public static string ToGoogleMapsLink(this GeoLocation geoLocation)
        => geoLocation.ToString().ToGoogleMapsLink(geoLocation.LatString, geoLocation.LonString);

    // https://stackoverflow.com/questions/1801732/how-do-i-link-to-google-maps-with-a-particular-longitude-and-latitude/52943975#52943975
    public static string ToGoogleMapsLink(this string value, string lat, string lon)
        => value.ToLink($"https://www.google.com/maps/search/?api=1&query={lat},{lon}");

    public static string ToLogLink(this string value, string logId)
        => value.ToLink(GroundspeakHelper.GetLogUrl(logId));

    private static string ValueToString(object value)
    {
        return value switch
        {
            double dbl => dbl.ToString("G29"),
            DateTime date => date.ToString("dd.MM.yyyy"),
            string str => str,
            _ => value.ToString() ?? "",
        };
    }
}