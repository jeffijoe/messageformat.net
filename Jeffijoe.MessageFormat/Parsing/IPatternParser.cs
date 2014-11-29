// IPatternParser.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// The pattern parser extracts patterns from a string.
    /// </summary>
    public interface IPatternParser
    {
        /// <summary>
        /// Parses the source, extracting formatter parameters
        /// describing what formatter to use, as well as it's options.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        IFormatterRequestCollection Parse(StringBuilder source);
    }
}