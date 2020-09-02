using System;
using System.Text;

namespace GCLogsAnalyzer
{
    public static class HtmlHelper
    {
        public static string TableRow(params object[] values)
        {
            return Row(values, false);
        }

        public static string TableRowHeader(params string[] values)
        {
            return Row(values, true);
        }

        private static string Row(object[] values, bool isHeader)
        {
            var buffer = new StringBuilder();
            buffer.Append("<tr>");
            var cellTag = isHeader ? "th" : "td";
            foreach (var value in values)
            {
                buffer.Append($"<{cellTag}>{ValueToString(value)}</{cellTag}>");
            }

            buffer.AppendLine("</tr>");
            return buffer.ToString();
        }

        public static string Headline(string name, string text, int level = 2) => $"<h{level} id=\"{name}\">{text}</h{level}>";

        /* Fixed Table-Header:
         https://css-tricks.com/position-sticky-and-table-headers/
         https://stackoverflow.com/questions/19559197/how-to-make-scrollable-table-with-fixed-headers-using-css
        */

        public const string PageHeader = @"<html>
<head>
<title>Founds</title>
<style>
body {
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
</style>
</head>
<body>";

        public static string TableHeader(string sectionName, string headerRow) => $"<div id=\"{sectionName}\" class=\"table\"><table><thead>{headerRow}</thead><tbody>";

        public const string TableFooter = "</tbody></table></div>";
            
        public const string PageFooter = "</body>\n</html>";

        public static string ToLink(this string text, string url, bool openInBlank = true)
        {
            return $"<a href=\"{url}\" {(openInBlank ? "target=\"_blank\"" : "")}>{text}</a>";
        }

        public static string ToGcUserLink(this string text, string ownerId)
        {
            return text.ToLink($"https://www.geocaching.com/p/default.aspx?id={ownerId}");
        }

        public static string ToGoogleMapsLink(this GeoLocation geoLocation)
        {
            return geoLocation.ToString().ToGoogleMapsLink(geoLocation.LatString, geoLocation.LonString);
        }

        public static string ToGoogleMapsLink(this string value, string lat, string lon)
        {
            // https://stackoverflow.com/questions/1801732/how-do-i-link-to-google-maps-with-a-particular-longitude-and-latitude/52943975#52943975

            return value.ToLink($"https://www.google.com/maps/search/?api=1&query={lat},{lon}");
        }

        private static string ValueToString(object value)
        {
            switch (value)
            {
                case double dbl:
                    return dbl.ToString("G29");
                case DateTime date:
                    return date.ToString("dd.MM.yyyy");
                case string str:
                    return str;
                default:
                    return value.ToString();
            }
        }
    }
}