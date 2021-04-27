// MessageFormat for .NET
// - LiteralParser.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    ///     Parser for extracting brace matches from a string builder.
    /// </summary>
    public class LiteralParser : ILiteralParser
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Finds the brace matches.
        /// </summary>
        /// <param name="sb">
        ///     The sb.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<Literal> ParseLiterals(StringBuilder sb)
        {
            const char OpenBrace = '{';
            const char CloseBrace = '}';
            const char EscapingChar = '\'';

            var result = new List<Literal>();
            var openBraces = 0;
            var closeBraces = 0;
            var start = 0;
            var braceBalance = 0;
            var lineNumber = 1;
            var startLineNumber = 1;
            var startColumnNumber = 0;
            var columnNumber = 0;
            var insideEscapeSequence = false;
            var currentEscapeSequenceLineNumber = 0;
            var currentEscapeSequenceColumnNumber = 0;
            const char Cr = '\r'; // Carriage return
            const char Lf = '\n'; // Line feed

            var matchTextBuf = StringBuilderPool.Get();
            try
            {
                for (var i = 0; i < sb.Length; i++)
                {
                    var c = sb[i];
                    if (c == Lf)
                    {
                        lineNumber++;
                        columnNumber = 0;
                        continue;
                    }

                    if (c == Cr)
                    {
                        continue;
                    }

                    columnNumber++;

                    if (c == EscapingChar)
                    {
                        if (i == sb.Length - 1)
                        {
                            if (!insideEscapeSequence)
                                matchTextBuf.Append(EscapingChar);

                            // The last char can't open a new escape sequence, it can only close one
                            if (insideEscapeSequence)
                            {
                                insideEscapeSequence = false;
                            }

                            continue;
                        }

                        matchTextBuf.Append(EscapingChar);

                        var nextChar = sb[i + 1];
                        if (nextChar == EscapingChar)
                        {
                            matchTextBuf.Append(EscapingChar);
                            ++i;
                            continue;
                        }

                        if (insideEscapeSequence)
                        {
                            insideEscapeSequence = false;
                            continue;
                        }

                        if (nextChar == '{' || nextChar == '}' || nextChar == '#')
                        {
                            matchTextBuf.Append(nextChar);
                            insideEscapeSequence = true;
                            currentEscapeSequenceLineNumber = lineNumber;
                            currentEscapeSequenceColumnNumber = columnNumber;
                            ++i;
                        }

                        continue;
                    }

                    if (insideEscapeSequence)
                    {
                        matchTextBuf.Append(c);
                        continue;
                    }

                    if (c == OpenBrace)
                    {
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

                    if (c == CloseBrace)
                    {
                        closeBraces++;
                        braceBalance--;

                        // Write the brace to the match buffer if it's not the closing brace
                        // we are looking for.
                        if (braceBalance > 0)
                        {
                            matchTextBuf.Append(c);
                        }
                    }
                    else
                    {
                        if (i > start && braceBalance > 0)
                        {
                            matchTextBuf.Append(c);
                        }

                        continue;
                    }

                    if (openBraces != closeBraces)
                    {
                        continue;
                    }

                    result.Add(new Literal(start, i, startLineNumber, startColumnNumber, matchTextBuf.ToString()));
                    matchTextBuf.Clear();
                    start = 0;
                }

                if (insideEscapeSequence)
                {
                    throw new MalformedLiteralException(
                        "There is an unclosed escape sequence.",
                        currentEscapeSequenceLineNumber,
                        currentEscapeSequenceColumnNumber,
                        matchTextBuf.ToString());
                }

                if (openBraces != closeBraces)
                {
                    throw new UnbalancedBracesException(openBraces, closeBraces);
                }

                return result;
            }
            finally
            {
                StringBuilderPool.Return(matchTextBuf);
            }
        }

        #endregion
    }
}