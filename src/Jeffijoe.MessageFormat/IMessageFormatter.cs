// MessageFormat for .NET
// - IMessageFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat;

/// <summary>
///     The magical Message Formatter.
/// </summary>
public interface IMessageFormatter
{
    #region Public properties

    /// <summary>
    ///     The custom value formatter to use for formats like `number`, `date`, `time` etc.
    /// </summary>
    CustomValueFormatter? CustomValueFormatter { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Formats the message with the specified arguments. It's so magical.
    /// </summary>
    /// <param name="pattern">
    ///     The pattern.
    /// </param>
    /// <param name="argsMap">
    ///     The arguments.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    string FormatMessage(string pattern, IReadOnlyDictionary<string, object?> argsMap);

    #endregion
}