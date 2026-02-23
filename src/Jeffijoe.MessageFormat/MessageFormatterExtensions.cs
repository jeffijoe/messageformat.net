using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Jeffijoe.MessageFormat.Helpers;

namespace Jeffijoe.MessageFormat;

/// <summary>
///     Extensions for <see cref="IMessageFormatter"/>.
/// </summary>
public static class MessageFormatterExtensions
{
    /// <summary>
    ///     Formats the message using the specified <paramref name="args"/>.
    /// </summary>
    /// <param name="formatter">
    ///     The formatter.
    /// </param>
    /// <param name="pattern">
    ///     The pattern.
    /// </param>
    /// <param name="args">
    ///     The arguments.
    /// </param>
    /// <param name="culture">
    ///     The culture to use, or <c>null</c> to use <see cref="CultureInfo.CurrentCulture"/>.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    public static string FormatMessage(
        this IMessageFormatter formatter,
        string pattern,
        IDictionary<string, object> args,
        CultureInfo? culture = null)
    {
        return formatter.FormatMessage(pattern, (IReadOnlyDictionary<string, object?>)args, culture);
    }

    /// <summary>
    ///     Formats the message, and uses reflection to create a dictionary of property values from the specified object.
    /// </summary>
    /// <param name="formatter">
    ///     The formatter.
    /// </param>
    /// <param name="pattern">
    ///     The pattern.
    /// </param>
    /// <param name="args">
    ///     The arguments.
    /// </param>
    /// <param name="culture">
    ///     The culture to use, or <c>null</c> to use <see cref="CultureInfo.CurrentCulture"/>.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    [OverloadResolutionPriority(-1)]
    [RequiresUnreferencedCode("This method uses the ToDictionary extension which uses reflection to convert object into dictionary")]
    public static string FormatMessage(this IMessageFormatter formatter, string pattern, object args, CultureInfo? culture = null)
    {
        return formatter.FormatMessage(pattern, args.ToDictionary(), culture);
    }
}
