// MessageFormat for .NET
// - IMessageFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat
{
    /// <summary>
    ///     The magical Message Formatter.
    /// </summary>
    public interface IMessageFormatter
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Formats the message with the specified arguments. It's so magical.
        /// </summary>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="argsMap">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string FormatMessage(string pattern, IDictionary<string, object> argsMap);

        /// <summary>
        ///     Formats the message, and uses reflection to create a dictionary of property values from the specified object.
        /// </summary>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string FormatMessage(string pattern, object args);

        #endregion
    }
}