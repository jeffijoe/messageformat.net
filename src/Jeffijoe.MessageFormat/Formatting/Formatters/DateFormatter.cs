using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters;

/// <summary>
///     Formatter for dates.
/// </summary>
public class DateFormatter : BaseValueFormatter, IFormatter
{
    /// <summary>
    ///     Name of this formatter.
    /// </summary>
    private const string FormatterName = "date";

    /// <inheritdoc />
    public override bool CanFormat(FormatterRequest request) =>
        request.FormatterName == FormatterName;

    /// <inheritdoc />
    protected override string FormatValue(
        CultureInfo culture,
        CustomValueFormatter? customValueFormatter,
        string variable,
        string style,
        object? value)
    {
        if (customValueFormatter?.TryFormatDate(culture, value, style, out var formatted) == true)
        {
            // When the formatter returns `true`, the string will be set.
            return formatted!;
        }

        return style switch
        {
            "" or "short" => string.Format(culture, "{0:d}", value),
            "full" => string.Format(culture, "{0:D}", value),
            _ => throw new UnsupportedFormatStyleException(
                variable: variable,
                format: FormatterName,
                style: style)
        };
    }
}