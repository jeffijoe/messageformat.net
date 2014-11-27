// Literal.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Text;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// Represents a position in the source text where we should look for format patterns.
    /// </summary>
    public class Literal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Literal" /> class.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <param name="sourceLineNumber">The source line number.</param>
        /// <param name="sourceColumnNumber">The source column number.</param>
        /// <param name="innerText">The inner text.</param>
        public Literal(
            int startIndex, 
            int endIndex, 
            int sourceLineNumber,
            int sourceColumnNumber,
            StringBuilder innerText)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            SourceLineNumber = sourceLineNumber;
            SourceColumnNumber = sourceColumnNumber;
            InnerText = innerText;
        }

        /// <summary>
        /// Gets or sets the start index in the source string.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets or sets the end index in the source string.
        /// </summary>
        /// <value>
        /// The end index.
        /// </value>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Gets the source line number in the original input string.
        /// </summary>
        /// <value>
        /// The source line number.
        /// </value>
        public int SourceLineNumber { get; private set; }

        /// <summary>
        /// Gets the source column number.
        /// </summary>
        /// <value>
        /// The source column number.
        /// </value>
        public int SourceColumnNumber { get; private set; }

        /// <summary>
        /// Gets or sets the inner text (the content between the braces).
        /// </summary>
        /// <value>
        /// The inner text.
        /// </value>
        public StringBuilder InnerText { get; set; }
    }
}