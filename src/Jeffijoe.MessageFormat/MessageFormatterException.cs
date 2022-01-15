// MessageFormat for .NET
// - MessageFormatterException.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;

namespace Jeffijoe.MessageFormat
{
    /// <summary>
    ///     Thrown when an issue has occured in the message formatting process.
    /// </summary>
    public class MessageFormatterException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageFormatterException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public MessageFormatterException(string message)
            : base(message)
        {
        }

        #endregion
    }
}