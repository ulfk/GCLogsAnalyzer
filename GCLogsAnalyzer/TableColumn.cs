using System;

namespace GCLogsAnalyzer;

public class TableColumn<T>
{
    public TableColumn(string headerText, Func<int, T, object> valueFunc)
    {
        HeaderText = headerText;
        ValueFunc = valueFunc;
    }

    public string HeaderText { get; }

    public Func<int, T, object> ValueFunc { get; }
}
