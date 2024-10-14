// MessageFormat for .NET
// - CharHelper.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.
namespace Jeffijoe.MessageFormat.Helpers;

/// <summary>
///     Char helper
/// </summary>
internal static class CharHelper
{
    #region Static Fields

    /// <summary>
    ///     The alphanumberic.
    /// </summary>
    private static readonly char[] Alphanumberic =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".ToCharArray();

    #endregion

    #region Methods

    /// <summary>
    ///     Determines whether the specified character is alpha numeric.
    /// </summary>
    /// <param name="c">
    ///     The c.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    internal static bool IsAlphaNumeric(this char c)
    {
        foreach (var chr in Alphanumberic)
        {
            if (chr == c)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}