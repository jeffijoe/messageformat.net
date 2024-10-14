// MessageFormat for .NET
// - FormatterRequest.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     Formatter request.
/// </summary>
public class FormatterRequest
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="FormatterRequest" /> class.
    /// </summary>
    /// <param name="sourceLiteral">
    ///     The source literal.
    /// </param>
    /// <param name="variable">
    ///     The variable.
    /// </param>
    /// <param name="formatterName">
    ///     Name of the formatter.
    /// </param>
    /// <param name="formatterArguments">
    ///     The formatter arguments.
    /// </param>
    public FormatterRequest(Literal sourceLiteral, string variable, string? formatterName, string? formatterArguments)
    {
        this.SourceLiteral = sourceLiteral;
        this.Variable = variable;
        this.FormatterName = formatterName;
        this.FormatterArguments = formatterArguments;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the formatter arguments that the formatter implementation will parse. Can be null.
    /// </summary>
    /// <value>
    ///     The formatter arguments.
    /// </value>
    public string? FormatterArguments { get; }

    /// <summary>
    ///     Gets the name of the formatter to use . e.g. 'select', 'plural'. Can be null.
    /// </summary>
    /// <value>
    ///     The name of the formatter.
    /// </value>
    public string? FormatterName { get; }

    /// <summary>
    ///     Gets the source literal.
    /// </summary>
    /// <value>
    ///     The source literal.
    /// </value>
    public Literal SourceLiteral { get; }

    /// <summary>
    ///     Gets the variable name. Never null.
    /// </summary>
    /// <value>
    ///     The variable.
    /// </value>
    public string Variable { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Clones this instance.
    /// </summary>
    /// <returns>
    ///     The <see cref="FormatterRequest" />.
    /// </returns>
    public FormatterRequest Clone()
    {
        return new FormatterRequest(
            this.SourceLiteral.Clone(), 
            this.Variable, 
            this.FormatterName, 
            this.FormatterArguments);
    }

    #endregion
}