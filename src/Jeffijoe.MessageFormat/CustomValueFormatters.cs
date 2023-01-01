using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Jeffijoe.MessageFormat;

/// <summary>
///     Attempts to format a date.
/// </summary>
/// <param name="culture">
///     The culture.
/// </param>
/// <param name="value">
///     The value to format.
/// </param>
/// <param name="style">
///     The requested style, if any.
/// </param>
/// <param name="formatted">
///     Output for setting the formatted result.
/// </param>
/// <returns>
///     <value>true</value> if able to format the value; <value>false</value> otherwise.
/// </returns>
public delegate bool TryFormatDate(
    CultureInfo culture,
    object? value,
    string? style,
    out string? formatted);

/// <summary>
///     Attempts to format a time.
/// </summary>
/// <param name="culture">
///     The culture.
/// </param>
/// <param name="value">
///     The value to format.
/// </param>
/// <param name="style">
///     The requested style, if any.
/// </param>
/// <param name="formatted">
///     Output for setting the formatted result.
/// </param>
/// <returns>
///     <value>true</value> if able to format the value; <value>false</value> otherwise.
/// </returns>
public delegate bool TryFormatTime(
    CultureInfo culture,
    object? value,
    string? style,
    out string? formatted);

/// <summary>
///     Attempts to format a number.
/// </summary>
/// <param name="culture">
///     The culture.
/// </param>
/// <param name="value">
///     The value to format.
/// </param>
/// <param name="style">
///     The requested style, if any.
/// </param>
/// <param name="formatted">
///     Output for setting the formatted result.
/// </param>
/// <returns>
///     <value>true</value> if able to format the value; <value>false</value> otherwise.
/// </returns>
public delegate bool TryFormatNumber(
    CultureInfo culture,
    object? value,
    string? style,
    out string? formatted);

/// <summary>
///     Base class that can be extended to provide custom formatting
///     for values.
/// </summary>
public abstract class CustomValueFormatter
{
    /// <summary>
    ///     Attempts to format a date.
    /// </summary>
    /// <param name="culture">
    ///     The culture.
    /// </param>
    /// <param name="value">
    ///     The value to format.
    /// </param>
    /// <param name="style">
    ///     The requested style, if any.
    /// </param>
    /// <param name="formatted">
    ///     Output for setting the formatted result.
    /// </param>
    /// <returns>
    ///     <value>true</value> if able to format the value; <value>false</value> otherwise.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public virtual bool TryFormatDate(
        CultureInfo culture,
        object? value,
        string? style,
        out string? formatted)
    {
        formatted = null;
        return false;
    }

    /// <summary>
    ///     Attempts to format a time.
    /// </summary>
    /// <param name="culture">
    ///     The culture.
    /// </param>
    /// <param name="value">
    ///     The value to format.
    /// </param>
    /// <param name="style">
    ///     The requested style, if any.
    /// </param>
    /// <param name="formatted">
    ///     Output for setting the formatted result.
    /// </param>
    /// <returns>
    ///     <value>true</value> if able to format the value; <value>false</value> otherwise.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public virtual bool TryFormatTime(
        CultureInfo culture,
        object? value,
        string? style,
        out string? formatted)
    {
        formatted = null;
        return false;
    }

    /// <summary>
    ///     Attempts to format a number.
    /// </summary>
    /// <param name="culture">
    ///     The culture.
    /// </param>
    /// <param name="value">
    ///     The value to format.
    /// </param>
    /// <param name="style">
    ///     The requested style, if any.
    /// </param>
    /// <param name="formatted">
    ///     Output for setting the formatted result.
    /// </param>
    /// <returns>
    ///     <value>true</value> if able to format the value; <value>false</value> otherwise.
    /// </returns>
    [ExcludeFromCodeCoverage]
    public virtual bool TryFormatNumber(
        CultureInfo culture,
        object? value,
        string? style,
        out string? formatted)
    {
        formatted = null;
        return false;
    }
}

/// <summary>
///     Delegates the formatting calls to the configured function properties.
/// </summary>
public sealed class CustomValueFormatters : CustomValueFormatter
{
    /// <summary>
    ///     Formatter for dates.
    /// </summary>
    public TryFormatDate? Date { get; set; }

    /// <summary>
    ///     Formatter for times.
    /// </summary>
    public TryFormatDate? Time { get; set; }

    /// <summary>
    ///     Formatter for numbers.
    /// </summary>
    public TryFormatNumber? Number { get; set; }

    /// <inheritdoc />
    public override bool TryFormatDate(CultureInfo culture, object? value, string? style,
        out string? formatted) =>
        this.Date?.Invoke(culture, value, style, out formatted) ??
        base.TryFormatDate(culture, value, style, out formatted);

    /// <inheritdoc />
    public override bool TryFormatTime(CultureInfo culture, object? value, string? style,
        out string? formatted) =>
        this.Time?.Invoke(culture, value, style, out formatted) ??
        base.TryFormatTime(culture, value, style, out formatted);

    /// <inheritdoc />
    public override bool TryFormatNumber(CultureInfo culture, object? value, string? style,
        out string? formatted) =>
        this.Number?.Invoke(culture, value, style, out formatted) ??
        base.TryFormatNumber(culture, value, style, out formatted);
}