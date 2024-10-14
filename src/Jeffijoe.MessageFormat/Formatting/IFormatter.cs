// MessageFormat for .NET
// - IFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     A Formatter is what transforms a pattern into a string, using the proper arguments.
/// </summary>
public interface IFormatter
{
    #region Public Properties

    /// <summary>
    ///     Each Formatter must declare whether or not an input variable is required to exist.
    ///     Most of the time that is the case. 
    /// </summary>
    bool VariableMustExist { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Determines whether this instance can format a message based on the specified parameters.
    /// </summary>
    /// <param name="request">
    ///     The parameters.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    bool CanFormat(FormatterRequest request);

    /// <summary>
    /// Using the specified parameters and arguments, a formatted string shall be returned.
    /// The <see cref="IMessageFormatter" /> is being provided as well, to enable
    /// nested formatting. This is only called if <see cref="CanFormat" /> returns true.
    /// The args will always contain the <see cref="FormatterRequest.Variable" />.
    /// </summary>
    /// <param name="locale">The locale being used. It is up to the formatter what they do with this information.</param>
    /// <param name="request">The parameters.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="value">The value of <see cref="FormatterRequest.Variable"/> from the given args dictionary. Can be null.</param>
    /// <param name="messageFormatter">The message formatter.</param>
    /// <returns>
    /// The <see cref="string" />.
    /// </returns>
    string Format(
        string locale,
        FormatterRequest request,
        IReadOnlyDictionary<string, object?> args,
        object? value,
        IMessageFormatter messageFormatter);

    #endregion
}