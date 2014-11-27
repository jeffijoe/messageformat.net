// UnbalancedBracesException.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// Thrown when the amount of open and close braces does not match.
    /// </summary>
    public class UnbalancedBracesException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnbalancedBracesException" /> class.
        /// </summary>
        /// <param name="openBraceCount">The brace counter.</param>
        /// <param name="closeBraceCount">The close brace count.</param>
        internal UnbalancedBracesException(int openBraceCount, int closeBraceCount) : base(BuildMessage(openBraceCount, closeBraceCount))
        {
            OpenBraceCount = openBraceCount;
            CloseBraceCount = closeBraceCount;
        }

        /// <summary>
        /// Gets the brace count.
        /// </summary>
        /// <value>
        /// The brace count.
        /// </value>
        public int OpenBraceCount { get; private set; }

        /// <summary>
        /// Gets the close brace count.
        /// </summary>
        /// <value>
        /// The close brace count.
        /// </value>
        public int CloseBraceCount { get; private set; }

        /// <summary>
        /// Builds the message.
        /// </summary>
        /// <param name="openBraceCount">The bracket counter.</param>
        /// <param name="closeBraceCount">The close brace count.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Bracket counter was 0, which would indicate success.</exception>
        private static string BuildMessage(int openBraceCount, int closeBraceCount)
        {
            if(openBraceCount == closeBraceCount) throw new ArgumentException("Bracket counter was 0, which would indicate success.");
            if (openBraceCount > closeBraceCount)
                return "There are " + (openBraceCount - closeBraceCount) + " more opening braces than there are closing braces.";
            return "There are " + (closeBraceCount - openBraceCount) + " more closing braces than there are opening braces.";
        }
    }
}