// MessageFormat for .NET
// - StringBuilderHelper.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

#if NET5_0_OR_GREATER
using System;
#endif

using System.Text;

namespace Jeffijoe.MessageFormat.Helpers;

/// <summary>
///     String Builder helper
/// </summary>
internal static class StringBuilderHelper
{
    #region Methods

    /// <summary>
    ///     Determines whether the specified source contains any of the specified characters.
    /// </summary>
    /// <param name="src">
    ///     The source.
    /// </param>
    /// <param name="chars">
    ///     The chars.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    private static bool Contains(this StringBuilder src, params char[] chars)
    {
#if NET5_0_OR_GREATER
        foreach (var chunk in src.GetChunks())
        {
            if (chunk.Span.IndexOfAny(chars) != -1)
            {
                return true;
            }
        }
#else
            for (int i = 0; i < src.Length; i++)
            {
                foreach (var c in chars)
                {
                    if (src[i] == c)
                    {
                        return true;
                    }
                }
            }
#endif

        return false;
    }
        
    /// <summary>
    ///     Determines whether the specified source contains the specified character.
    /// </summary>
    /// <param name="src">
    ///     The source.
    /// </param>
    /// <param name="character">
    ///     The character.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    internal static bool Contains(this StringBuilder src, char character)
    {
#if NET5_0_OR_GREATER
        foreach (var chunk in src.GetChunks())
        {
            if (chunk.Span.IndexOf(character) != -1)
                return true;
        }
#else
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == character)
                {
                    return true;
                }
            }
#endif

        return false;
    }

    /// <summary>
    ///     Determines whether the specified source contains whitespace.
    /// </summary>
    /// <param name="src">
    ///     The source.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    internal static bool ContainsWhitespace(this StringBuilder src)
    {
        return src.Contains(' ', '\r', '\n', '\t');
    }

    /// <summary>
    ///     Trims the whitespace.
    /// </summary>
    /// <param name="src">
    ///     The source.
    /// </param>
    /// <returns>
    ///     The <see cref="StringBuilder" />.
    /// </returns>
    internal static StringBuilder TrimWhitespace(this StringBuilder src)
    {
        var length = 0;

        for (int i = 0; i < src.Length; i++)
        {
            var c = src[i];
            if (char.IsWhiteSpace(c) == false)
            {
                length = i;
                break;
            }
        }

        if (length != 0)
        {
            src = src.Remove(0, length);
        }

        var startIndex = 0;
        for (int i = src.Length - 1; i >= 0; i--)
        {
            var c = src[i];
            if (char.IsWhiteSpace(c) == false)
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == src.Length)
        {
            return src;
        }

        length = src.Length - startIndex;
        src.Remove(startIndex, length);
        return src;
    }

    #endregion
}