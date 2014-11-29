// IMessageFormatter.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat
{
    /// <summary>
    /// The magical Message Formatter.
    /// </summary>
    public interface IMessageFormatter
    {
        /// <summary>
        /// Formats the message with the specified arguments. It's so magical.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        string FormatMessage(string pattern, Dictionary<string, object> args);
    }
}