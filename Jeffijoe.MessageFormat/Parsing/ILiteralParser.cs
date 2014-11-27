// ILiteralParser.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;
using System.Text;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// Brace parser contract.
    /// </summary>
    public interface ILiteralParser
    {
        /// <summary>
        /// Finds the brace matches.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <returns></returns>
        /// <exception cref="UnbalancedBracesException"></exception>
        IEnumerable<Literal> ParseLiterals(StringBuilder sb);
    }
}