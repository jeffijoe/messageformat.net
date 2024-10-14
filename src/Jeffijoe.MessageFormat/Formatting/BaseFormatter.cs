// MessageFormat for .NET
// - BaseFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    ///     Base formatter with helpers for extracting data from the formatter request.
    /// </summary>
    public abstract class BaseFormatter
    {
        #region Constants

        /// <summary>
        ///     The other.
        /// </summary>
        protected const string OtherKey = "other";

        #endregion

        #region Methods

        /// <summary>
        ///     Parses the arguments.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <returns>
        ///     The <see cref="ParsedArguments" />.
        /// </returns>
        protected internal ParsedArguments ParseArguments(FormatterRequest request)
        {
            int index;
            var extensions = this.ParseExtensions(request, out index);
            var keyedBlocks = this.ParseKeyedBlocks(request, index);
            return new ParsedArguments(keyedBlocks, extensions);
        }

        /// <summary>
        ///     Parses the extensions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="index">The index.</param>
        /// <returns>The formatter extensions.</returns>
        protected internal IEnumerable<FormatterExtension> ParseExtensions(FormatterRequest request, out int index)
        {
            var result = new List<FormatterExtension>();
            if (request.FormatterArguments == null)
            {
                index = -1;
                return Enumerable.Empty<FormatterExtension>();
            }
            
            var length = request.FormatterArguments.Length;
            index = 0;
            const char Colon = ':';
            const char OpenBrace = '{';
            var foundExtension = false;

            var extension = StringBuilderPool.Get();
            var value = StringBuilderPool.Get();
            try
            {
                for (var i = 0; i < length; i++)
                {
                    var c = request.FormatterArguments[i];

                    // Whitespace is tolerated at the beginning.
                    var isWhiteSpace = char.IsWhiteSpace(c);
                    if (isWhiteSpace)
                    {
                        // We've reached the end
                        if (value.Length > 0)
                        {
                            foundExtension = false;
                            result.Add(new FormatterExtension(extension.ToString(), value.ToString()));
                            extension.Clear();
                            value.Clear();
                            index = i;
                            continue;
                        }

                        if (extension.Length > 0)
                        {
                            // It's not an extension, so we're done looking.
                            break;
                        }

                        continue;
                    }

                    if (c == Colon)
                    {
                        foundExtension = true;
                        continue;
                    }

                    if (foundExtension)
                    {
                        value.Append(c);
                        continue;
                    }

                    if (c == OpenBrace)
                    {
                        // It's not an extension.
                        break;
                    }

                    extension.Append(c);
                }

                return result;
            }
            finally
            {
                StringBuilderPool.Return(extension);
                StringBuilderPool.Return(value);
            }
        }

        /// <summary>
        ///     Parses the keyed blocks.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <param name="startIndex">
        ///     The start index.
        /// </param>
        /// <returns>
        ///     The keyed blocks.
        /// </returns>
        protected internal IEnumerable<KeyedBlock> ParseKeyedBlocks(FormatterRequest request, int startIndex)
        {
            const char OpenBrace = '{';
            const char CloseBrace = '}';
            const char EscapingChar = '\'';

            var result = new List<KeyedBlock>();
            var braceBalance = 0;
            var foundWhitespaceAfterKey = false;
            var insideEscapeSequence = false;
            if (request.FormatterArguments == null)
            {
                return Enumerable.Empty<KeyedBlock>();
            }

            var key = StringBuilderPool.Get();
            var block = StringBuilderPool.Get();

            try
            {
                for (int i = startIndex; i < request.FormatterArguments.Length; i++)
                {
                    var c = request.FormatterArguments[i];
                    var isWhitespace = char.IsWhiteSpace(c);

                    if (c == EscapingChar)
                    {
                        if (braceBalance == 0)
                        {
                            throw new MalformedLiteralException(
                                "Expected a key, but found start of a escape sequence.",
                                0,
                                0,
                                request.FormatterArguments);
                        }

                        if (i == request.FormatterArguments.Length - 1)
                        {
                            if (!insideEscapeSequence)
                                block.Append(EscapingChar);

                            // The last char can't open a new escape sequence, it can only close one
                            if (insideEscapeSequence)
                            {
                                insideEscapeSequence = false;
                            }

                            continue;
                        }

                        var nextChar = request.FormatterArguments[i + 1];
                        if (nextChar == EscapingChar)
                        {
                            block.Append(EscapingChar);
                            block.Append(EscapingChar);
                            ++i;
                            continue;
                        }

                        if (insideEscapeSequence)
                        {
                            block.Append(EscapingChar);
                            insideEscapeSequence = false;
                            continue;
                        }

                        if (nextChar == '{' || nextChar == '}' || nextChar == '#')
                        {
                            block.Append(EscapingChar);
                            block.Append(nextChar);
                            insideEscapeSequence = true;
                            ++i;
                            continue;
                        }

                        block.Append(EscapingChar);
                        continue;
                    }

                    if (insideEscapeSequence)
                    {
                        block.Append(c);
                        continue;
                    }

                    if (c == OpenBrace)
                    {
                        if (key.Length == 0)
                        {
                            throw new MalformedLiteralException(
                                "Expected a key, but found start of a new block.",
                                0,
                                0,
                                request.FormatterArguments);
                        }

                        braceBalance++;
                        if (braceBalance > 1)
                        {
                            block.Append(c);
                        }

                        continue;
                    }

                    if (c == CloseBrace)
                    {
                        if (key.Length == 0)
                        {
                            throw new MalformedLiteralException(
                                "Expected a key, but found end of a block.",
                                0,
                                0,
                                request.FormatterArguments);
                        }

                        if (braceBalance == 0)
                        {
                            throw new MalformedLiteralException(
                                "Found end of a block, but no block has been started, or the"
                                + " block has already been closed. " +
                                "This could indicate an unescaped brace somewhere.",
                                0,
                                0,
                                request.FormatterArguments);
                        }

                        braceBalance--;
                        if (braceBalance == 0)
                        {
                            result.Add(new KeyedBlock(key.ToString(), block.ToString()));
                            block.Clear();
                            key.Clear();
                            foundWhitespaceAfterKey = false;
                            continue;
                        }
                    }

                    // If we are inside a block, append to the block buffer
                    if (braceBalance > 0)
                    {
                        block.Append(c);
                        continue;
                    }

                    // Else, we are buffering our key
                    if (isWhitespace == false)
                    {
                        if (foundWhitespaceAfterKey)
                        {
                            throw new MalformedLiteralException(
                                "Any whitespace after a key should be followed by the beginning of a block.",
                                0,
                                0,
                                request.FormatterArguments);
                        }

                        key.Append(c);
                    }
                    else if (key.Length > 0)
                    {
                        foundWhitespaceAfterKey = true;
                    }
                }

                if (insideEscapeSequence)
                {
                    throw new MalformedLiteralException(
                        "There is an unclosed escape sequence.",
                        0,
                        0,
                        request.FormatterArguments);
                }

                if (braceBalance > 0)
                {
                    throw new MalformedLiteralException(
                        "There are more open braces than there are close braces.",
                        0,
                        0,
                        request.FormatterArguments);
                }

                return result;
            }
            finally
            {
                StringBuilderPool.Return(key);
                StringBuilderPool.Return(block);
            }
        }

        #endregion
    }
}