using System;
using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters;

/// <summary>
///     Formatter for numbers.
/// </summary>
public class NumberFormatter : BaseValueFormatter, IFormatter
{
    /// <summary>
    ///     Name of this formatter.
    /// </summary>
    private const string FormatterName = "number";

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
        if (customValueFormatter?.TryFormatNumber(culture, value, style, out var formatted) == true)
        {
            // When the formatter returns `true`, the string will be set.
            return formatted!;
        }

        return style switch
        {
            "" => string.Format(culture, "{0:#,##0.###}", value),
            "integer" => FormatInteger(culture, value),
            "currency" => string.Format(culture, "{0:C}", value),
            "percent" => string.Format(culture, "{0:P0}", value),
            _ => throw new UnsupportedFormatStyleException(
                variable: variable,
                format: FormatterName,
                style: style)
        };
    }

    /// <summary>
    ///     Attempts to format as an integer by first converting the value to
    ///     an integer. Otherwise prints the value as-is.
    /// </summary>
    /// <param name="cultureInfo"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string FormatInteger(IFormatProvider cultureInfo, object? value) =>
        value switch
        {
            decimal or float or double => string.Format(cultureInfo, "{0}", Convert.ToInt64(value)),
            string s => decimal.TryParse(s, NumberStyles.Any, cultureInfo, out var parsed) ? FormatInteger(cultureInfo, parsed) : s,
            _ => string.Format(cultureInfo, "{0}", value)
        };
}