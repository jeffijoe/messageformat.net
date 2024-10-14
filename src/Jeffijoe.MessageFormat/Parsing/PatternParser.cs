// MessageFormat for .NET
// - PatternParser.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

#if NET5_0_OR_GREATER
using System;
#endif
using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Helpers;

namespace Jeffijoe.MessageFormat.Parsing;

/// <summary>
///     Parser for extracting formatter patterns.
/// </summary>
public class PatternParser : IPatternParser
{
    #region Fields

    /// <summary>
    ///     The _literal parser.
    /// </summary>
    private readonly ILiteralParser literalParser;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="PatternParser" /> class.
    /// </summary>
    public PatternParser() : this(new LiteralParser())
    {
    }
        
    /// <summary>
    ///     Initializes a new instance of the <see cref="PatternParser" /> class.
    /// </summary>
    /// <param name="literalParser">
    ///     The literal parser.
    /// </param>
    public PatternParser(ILiteralParser literalParser)
    {
        this.literalParser = literalParser;
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Parses the specified source.
    /// </summary>
    /// <param name="source">
    ///     The source.
    /// </param>
    /// <returns>
    ///     The <see cref="IFormatterRequestCollection" />.
    /// </returns>
    public IFormatterRequestCollection Parse(StringBuilder source)
    {
        var literals = this.literalParser.ParseLiterals(source).ToArray();
        if (literals.Length == 0)
        {
            return new FormatterRequestCollection();
        }

        var result = new FormatterRequestCollection();
        foreach (var literal in literals)
        {
            // The first token to follow an opening brace will be the variable name.
            var variableName = ReadLiteralSection(literal, 0, false, out var lastIndex)!;

            // The next (if any), is the formatter to use. Null is allowed.
            string? formatterKey = null;

            // The rest of the string is what we pass into the formatter. Can be null.
            string? formatterArgs = null;
            if (variableName.Length != literal.InnerText.Length)
            {
                formatterKey = ReadLiteralSection(literal, lastIndex + 1, true, out lastIndex);
                if (formatterKey != null)
                {
#if NET5_0_OR_GREATER
                        formatterArgs =
                            literal.InnerText.AsSpan(lastIndex + 1, literal.InnerText.Length - lastIndex - 1).Trim()
                                .ToString();
#else
                    formatterArgs =
                        literal.InnerText.Substring(lastIndex + 1, literal.InnerText.Length - lastIndex - 1).Trim();
#endif
                }
            }

            result.Add(new FormatterRequest(literal, variableName, formatterKey, formatterArgs));
        }

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Gets the key from the literal.
    /// </summary>
    /// <param name="literal">
    ///     The literal.
    /// </param>
    /// <param name="offset">
    ///     The offset.
    /// </param>
    /// <param name="allowEmptyResult">
    ///     if set to <c>true</c>, allows an empty result, in which case the return value is
    ///     <c>null</c>
    /// </param>
    /// <param name="lastIndex">
    ///     The last index.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    /// <exception cref="MalformedLiteralException">
    ///     Parsing the variable key yielded an empty string.
    /// </exception>
    internal static string? ReadLiteralSection(Literal literal, int offset, bool allowEmptyResult,
        out int lastIndex)
    {
        const char Comma = ',';

        var innerText = literal.InnerText;
        var column = literal.SourceColumnNumber;
        var foundWhitespace = false;
        lastIndex = 0;
        var sb = StringBuilderPool.Get();
        try
        {
            for (var i = offset; i < innerText.Length; i++)
            {
                var c = innerText[i];
                column++;
                lastIndex = i;
                if (c == Comma)
                {
                    break;
                }

                // Disregard whitespace.
                var whitespace = char.IsWhiteSpace(c);
                if (!whitespace)
                {
                    if (c.IsAlphaNumeric() == false)
                    {
                        var msg = $"Invalid literal character '{c}'.";

                        // Line number can't have changed.
                        throw new MalformedLiteralException(
                            msg, 
                            literal.SourceLineNumber, 
                            column,
                            innerText);
                    }
                }
                else
                {
                    foundWhitespace = true;
                }

                sb.Append(c);
            }

            if (sb.Length != 0)
            {
                // Trim whitespace from beginning and end of the string, if necessary.
                if (!foundWhitespace)
                {
                    return sb.ToString();
                }

                StringBuilder trimmed = sb.TrimWhitespace();
                if (trimmed.Length == 0)
                {
                    if (allowEmptyResult)
                    {
                        return null;
                    }

                    throw new MalformedLiteralException(
                        "Parsing the literal yielded a string that was pure whitespace.",
                        literal.SourceLineNumber,
                        column);
                }

                if (trimmed.ContainsWhitespace())
                {
                    throw new MalformedLiteralException(
                        "Parsed literal must not contain whitespace.",
                        0,
                        0,
                        trimmed.ToString());
                }

                return sb.ToString();
            }

            if (allowEmptyResult)
            {
                return null;
            }

            throw new MalformedLiteralException(
                "Parsing the literal yielded an empty string.",
                literal.SourceLineNumber,
                column);
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }

    #endregion
}