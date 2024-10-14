// MessageFormat for .NET
// - FormatterExtension.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.
namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     Contains extensions to be used by formatters.
///     Example, the offset extension for the Plural Format.
/// </summary>
public class FormatterExtension
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="FormatterExtension" /> class.
    /// </summary>
    /// <param name="extension">
    ///     The extension.
    /// </param>
    /// <param name="value">
    ///     The value.
    /// </param>
    public FormatterExtension(string extension, string value)
    {
        this.Extension = extension;
        this.Value = value;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the extension.
    /// </summary>
    /// <value>
    ///     The extension.
    /// </value>
    public string Extension { get; private set; }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    /// <value>
    ///     The value.
    /// </value>
    public string Value { get; private set; }

    #endregion
}