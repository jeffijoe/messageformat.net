// MalformedLiteralException.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// Thrown when the pattern parser finds an invalid character in a literal.
    /// </summary>
    public class MalformedLiteralException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MalformedLiteralException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="sourceSnippet">A snippet of the text that contained the error. Can be null.</param>
        internal MalformedLiteralException(
            string message, 
            int lineNumber = 0, 
            int columnNumber = 0, 
            string sourceSnippet = null) : base(BuildMessage(message, lineNumber, columnNumber, sourceSnippet))
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        /// <value>
        /// The line number.
        /// </value>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        /// <value>
        /// The column number.
        /// </value>
        public int ColumnNumber { get; private set; }

        /// <summary>
        /// Gets or sets the source snippet.
        /// </summary>
        /// <value>
        /// The source snippet.
        /// </value>
        public string SourceSnippet { get; private set; }

        /// <summary>
        /// Builds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="sourceSnippet">The source snippet.</param>
        /// <returns></returns>
        private static string BuildMessage(string message, int lineNumber, int columnNumber, string sourceSnippet)
        {
            var str = string.Empty;
            if(lineNumber != 0 && columnNumber != 0)
            {
                str = string.Format("{0}\r\nLine {1}, column {2}", message, lineNumber, columnNumber);
            }
            if (string.IsNullOrWhiteSpace(sourceSnippet))
                return str;
            return string.Format("{0}\r\nOffending snippet: \"{1}\"", str, sourceSnippet);
        }
    }
}