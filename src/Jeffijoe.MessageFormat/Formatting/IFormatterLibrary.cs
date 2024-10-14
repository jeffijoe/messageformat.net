// MessageFormat for .NET
// - IFormatterLibrary.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     Manages formatters to use.
/// </summary>
public interface IFormatterLibrary : IList<IFormatter>
{
    #region Public Methods and Operators

    /// <summary>
    ///     Gets the formatter to use. If none was found, throws an exception.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <returns>
    ///     The <see cref="IFormatter" />.
    /// </returns>
    IFormatter GetFormatter(FormatterRequest request);

    #endregion
}