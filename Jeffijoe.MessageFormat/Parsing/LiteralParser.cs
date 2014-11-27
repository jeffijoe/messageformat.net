// LiteralParser.cs
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
    /// Parser for extracting brace matches from a string builder.
    /// </summary>
    public class LiteralParser : ILiteralParser
    {
        /// <summary>
        /// Finds the brace matches.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <returns></returns>
        /// <exception cref="UnbalancedBracesException"></exception>
        public IEnumerable<Literal> ParseLiterals(StringBuilder sb)
        {
            const char openBrace = '{';
            const char closeBrace = '}';
            const char escapingBackslash = '\\'; // It's just a single \

            var result = new List<Literal>();
            var openBraces = 0;
            var closeBraces = 0;
            var start = 0;
            var braceBalance = 0;
            var matchTextBuf = new StringBuilder();
            var lineNumber = 1;
            var startLineNumber = 1;
            var startColumnNumber = 0;
            var columnNumber = 0;
            const char LF = '\n'; // Line feed
            for (var i = 0; i < sb.Length; i++)
            {
                var c = sb[i];
                if (c == LF)
                {
                    lineNumber++;
                    columnNumber = 0;
                    continue;
                }
                columnNumber++;

                if(c == openBrace)
                {
                    // Don't check for escaping when we're at the first char.
                    if (i != 0)
                    {
                        if (sb[i - 1] == escapingBackslash) 
                            continue;
                    }
                    openBraces++;
                    braceBalance++;
                    // Record starting position of possible new brace match.
                    if (braceBalance == 1)
                    {
                        start = i;
                        startColumnNumber = columnNumber;
                        startLineNumber = lineNumber;
                        matchTextBuf.Clear();
                    }
                }

                if (c == closeBrace)
                {
                    // Don't check for escaping when we're at the first char.
                    if (i != 0)
                    {
                        if (sb[i - 1] == escapingBackslash)
                            continue;
                    }
                    closeBraces++;
                    braceBalance--;
                    // Write the brace to the match buffer if it's not the closing brace
                    // we are looking for.
                    if (braceBalance > 0)
                        matchTextBuf.Append(c);
                } 
                else
                {
                    if (i > start && braceBalance > 0)
                    {
                        matchTextBuf.Append(c);
                    }
                    continue;
                }


                if (openBraces != closeBraces) continue;
                // Passing in the text buffer instead of the actual string to void allocating a new string.
                result.Add(new Literal(start, i, startLineNumber, startColumnNumber, matchTextBuf));
                matchTextBuf = new StringBuilder();
                start = 0;
            }
            if(openBraces != closeBraces)
                throw new UnbalancedBracesException(openBraces, closeBraces);
            return result;
        }
    }
}