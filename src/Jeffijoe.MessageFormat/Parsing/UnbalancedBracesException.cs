// MessageFormat for .NET
// - UnbalancedBracesException.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;

namespace Jeffijoe.MessageFormat.Parsing;

/// <summary>
///     Thrown when the amount of open and close braces does not match.
/// </summary>
public class UnbalancedBracesException : ArgumentException
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnbalancedBracesException" /> class.
    /// </summary>
    /// <param name="openBraceCount">
    ///     The brace counter.
    /// </param>
    /// <param name="closeBraceCount">
    ///     The close brace count.
    /// </param>
    internal UnbalancedBracesException(int openBraceCount, int closeBraceCount)
        : base(BuildMessage(openBraceCount, closeBraceCount))
    {
        this.OpenBraceCount = openBraceCount;
        this.CloseBraceCount = closeBraceCount;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the close brace count.
    /// </summary>
    /// <value>
    ///     The close brace count.
    /// </value>
    public int CloseBraceCount { get; private set; }

    /// <summary>
    ///     Gets the brace count.
    /// </summary>
    /// <value>
    ///     The brace count.
    /// </value>
    public int OpenBraceCount { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    ///     Builds the message.
    /// </summary>
    /// <param name="openBraceCount">
    ///     The bracket counter.
    /// </param>
    /// <param name="closeBraceCount">
    ///     The close brace count.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    ///     Bracket counter was 0, which would indicate success.
    /// </exception>
    private static string BuildMessage(int openBraceCount, int closeBraceCount)
    {
        if (openBraceCount > closeBraceCount)
        {
            return "There are " + (openBraceCount - closeBraceCount)
                                + " more opening braces than there are closing braces.";
        }

        return "There are " + (closeBraceCount - openBraceCount)
                            + " more closing braces than there are opening braces.";
    }

    #endregion
}