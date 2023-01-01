using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters;

/// <summary>
///     Base formatter for values such as numbers, dates, times, etc.
/// </summary>
public abstract class BaseValueFormatter : BaseFormatter, IFormatter
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseValueFormatter" /> class.
    /// </summary>
    protected BaseValueFormatter()
    {
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool VariableMustExist => true;

    /// <inheritdoc />
    public abstract bool CanFormat(FormatterRequest request);

    /// <summary>
    ///     Formats the value using the given style.
    /// </summary>
    /// <param name="culture"></param>
    /// <param name="customValueFormatter"></param>
    /// <param name="variable"></param>
    /// <param name="style"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected abstract string FormatValue(CultureInfo culture, CustomValueFormatter? customValueFormatter,
        string variable, string style, object? value);

    /// <inheritdoc />
    public string Format(
        string locale,
        FormatterRequest request,
        IReadOnlyDictionary<string, object?> args,
        object? value,
        IMessageFormatter messageFormatter)
    {
        var formatterArgs = request.FormatterArguments!;
        var culture = CultureInfo.GetCultureInfo(locale);
        return FormatValue(
            culture: culture,
            customValueFormatter: messageFormatter.CustomValueFormatter,
            variable: request.Variable,
            style: formatterArgs,
            value: value);
    }
}