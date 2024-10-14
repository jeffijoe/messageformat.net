// MessageFormat for .NET
// - IPatternParser.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Text;

namespace Jeffijoe.MessageFormat.Parsing;

/// <summary>
///     The pattern parser extracts patterns from a string.
/// </summary>
public interface IPatternParser
{
    #region Public Methods and Operators

    /// <summary>
    ///     Parses the source, extracting formatter parameters
    ///     describing what formatter to use, as well as it's options.
    /// </summary>
    /// <param name="source">
    ///     The source.
    /// </param>
    /// <returns>
    ///     The <see cref="IFormatterRequestCollection" />.
    /// </returns>
    IFormatterRequestCollection Parse(StringBuilder source);

    #endregion
}