// MessageFormat for .NET
// - KeyedBlock.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.
namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     A keyed block contains a key and a block
///     containing the text that the formatter will return
///     when the block is being used.
/// </summary>
public class KeyedBlock
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyedBlock" /> class.
    /// </summary>
    /// <param name="key">
    ///     The key.
    /// </param>
    /// <param name="blockText">
    ///     The block text.
    /// </param>
    public KeyedBlock(string key, string blockText)
    {
        this.Key = key;
        this.BlockText = blockText;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the block text to be returned by the formatter.
    /// </summary>
    /// <value>
    ///     The block text.
    /// </value>
    public string BlockText { get; private set; }

    /// <summary>
    ///     Gets the key used by the formatter to make decisions.
    /// </summary>
    /// <value>
    ///     The key.
    /// </value>
    public string Key { get; private set; }

    #endregion
}