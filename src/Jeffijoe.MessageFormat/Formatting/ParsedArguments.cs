// MessageFormat for .NET
// - ParsedArguments.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     Container class for formatter argument parsing result.
/// </summary>
public class ParsedArguments
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParsedArguments" /> class.
    /// </summary>
    /// <param name="keyedBlocks">
    ///     The keyed Blocks.
    /// </param>
    /// <param name="extensions">
    ///     The extensions.
    /// </param>
    public ParsedArguments(IEnumerable<KeyedBlock> keyedBlocks, IEnumerable<FormatterExtension> extensions)
    {
        this.KeyedBlocks = keyedBlocks.ToList();
        this.Extensions = extensions.ToList();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the extensions.
    /// </summary>
    /// <value>
    ///     The extensions.
    /// </value>
    public IEnumerable<FormatterExtension> Extensions { get; private set; }

    /// <summary>
    ///     Gets the keyed blocks.
    /// </summary>
    /// <value>
    ///     The keyed blocks.
    /// </value>
    public IEnumerable<KeyedBlock> KeyedBlocks { get; private set; }

    #endregion
}