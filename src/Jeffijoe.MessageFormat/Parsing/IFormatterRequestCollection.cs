// MessageFormat for .NET
// - IFormatterRequestCollection.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat.Parsing;

/// <summary>
///     Formatter requests collection.
/// </summary>
public interface IFormatterRequestCollection : IReadOnlyList<FormatterRequest>
{
    #region Public Methods and Operators

    /// <summary>
    ///     Clones this instance and all of it's items. This lets us reuse pattern parsing result, without having to remember
    ///     the item's initial state before being modified to match the results of the formatters.
    /// </summary>
    /// <returns>
    ///     The <see cref="IFormatterRequestCollection" />.
    /// </returns>
    IFormatterRequestCollection Clone();

    /// <summary>
    ///     Updates the indices of all
    ///     formatter requests' source literals, starting at
    ///     the specified index in this collection.
    /// </summary>
    /// <param name="indexToStartFrom">
    ///     The index to start from.
    /// </param>
    /// <param name="formatterResultLength">
    ///     Length of the formatter result.
    ///     Used to compare each literal's inner text length, so we know what to set the
    ///     indices to on the rest of the requests.
    /// </param>
    void ShiftIndices(int indexToStartFrom, int formatterResultLength);

    #endregion
}