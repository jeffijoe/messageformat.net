// MessageFormat for .NET
// - Literal.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    ///     Represents a position in the source text where we should look for format patterns.
    /// </summary>
    public class Literal
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Literal" /> class.
        /// </summary>
        /// <param name="startIndex">
        ///     The start index.
        /// </param>
        /// <param name="endIndex">
        ///     The end index.
        /// </param>
        /// <param name="sourceLineNumber">
        ///     The source line number.
        /// </param>
        /// <param name="sourceColumnNumber">
        ///     The source column number.
        /// </param>
        /// <param name="innerText">
        ///     The inner text.
        /// </param>
        public Literal(
            int startIndex, 
            int endIndex, 
            int sourceLineNumber, 
            int sourceColumnNumber, 
            string innerText)
        {
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.SourceLineNumber = sourceLineNumber;
            this.SourceColumnNumber = sourceColumnNumber;
            this.InnerText = innerText;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the end index in the source string.
        /// </summary>
        /// <value>
        ///     The end index.
        /// </value>
        public int EndIndex { get; private set; }

        /// <summary>
        ///     Gets the inner text (the content between the braces).
        /// </summary>
        /// <value>
        ///     The inner text.
        /// </value>
        public string InnerText { get; private set; }

        /// <summary>
        ///     Gets the source column number.
        /// </summary>
        /// <value>
        ///     The source column number.
        /// </value>
        public int SourceColumnNumber { get; private set; }

        /// <summary>
        ///     Gets the source line number in the original input string.
        /// </summary>
        /// <value>
        ///     The source line number.
        /// </value>
        public int SourceLineNumber { get; private set; }

        /// <summary>
        ///     Gets the start index in the source string.
        /// </summary>
        /// <value>
        ///     The start index.
        /// </value>
        public int StartIndex { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        /// <returns>
        ///     The <see cref="Literal" />.
        /// </returns>
        public Literal Clone()
        {
            // Assuming that InnerText will never be tampered with.
            return new Literal(
                this.StartIndex, 
                this.EndIndex, 
                this.SourceLineNumber, 
                this.SourceColumnNumber, 
                this.InnerText);
        }

        /// <summary>
        ///     Updates the start and end index.
        /// </summary>
        /// <param name="resultLength">
        ///     Length of the result.
        /// </param>
        /// <param name="literal">
        ///     The literal that was just formatted.
        /// </param>
        public void ShiftIndices(int resultLength, Literal literal)
        {
            int offset = (literal.EndIndex - literal.StartIndex) - 1;
            this.StartIndex = (this.StartIndex - offset) + resultLength;
            this.EndIndex = (this.EndIndex - offset) + resultLength;
        }

        #endregion
    }
}