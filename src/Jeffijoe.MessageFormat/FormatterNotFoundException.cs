// MessageFormat for .NET
// - FormatterNotFoundException.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;

using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat;

/// <summary>
///     Thrown when a formatter could not be found for a specific request.
/// </summary>
public class FormatterNotFoundException : Exception
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="FormatterNotFoundException" /> class.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    public FormatterNotFoundException(FormatterRequest request)
        : base(BuildMessage(request))
    {
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Builds the message.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    private static string BuildMessage(FormatterRequest request)
    {
        return
            string.Format(
                "Format '{0}' could not be resolved.\r\n" + "Line {1}, position {2}\r\n" + "Source literal: '{3}'", 
                request.FormatterName, 
                request.SourceLiteral.SourceLineNumber, 
                request.SourceLiteral.SourceColumnNumber, 
                request.SourceLiteral.InnerText);
    }

    #endregion
}