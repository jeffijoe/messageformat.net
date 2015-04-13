// MessageFormat for .NET
// - Pluralizer.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.
namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    ///     Given the specified number, determines what plural form is being used.
    /// </summary>
    /// <param name="n">The number used to determine the pluralization rule..</param>
    /// <returns>The plural form to use.</returns>
    public delegate string Pluralizer(double n);
}