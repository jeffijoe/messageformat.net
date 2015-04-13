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

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageFormatterException" /> class.
        /// </summary>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public MessageFormatterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}