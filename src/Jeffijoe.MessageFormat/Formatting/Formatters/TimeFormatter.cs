using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters;

/// <summary>
///     Formatter for times.
/// </summary>
public class TimeFormatter : BaseValueFormatter, IFormatter
{
    /// <summary>
    ///     Name of this formatter.
    /// </summary>
    private const string FormatterName = "time";

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
        if (customValueFormatter?.TryFormatTime(culture, value, style, out var formatted) == true)
        {
            // When the formatter returns `true`, the string will be set.
            return formatted!;
        }

        return style switch
        {
            "" or "medium" => string.Format(culture, "{0:T}", value),
            "short" => string.Format(culture, "{0:t}", value),
            _ => throw new UnsupportedFormatStyleException(
                variable: variable,
                format: FormatterName,
                style: style)
        };
    }
}