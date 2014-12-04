using System;
using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Base formatter with helpers for extracting data from the formatter request.
    /// </summary>
    public abstract class BaseFormatter
    {
        protected const string Other = "other";

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        protected internal ParsedArguments ParseArguments(FormatterRequest request)
        {
            int index;
            var extensions = ParseExtensions(request, out index);
            var keyedBlocks = ParseKeyedBlocks(request, index);
            return new ParsedArguments(keyedBlocks, extensions);
        }

        /// <summary>
        /// Parses the extensions.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected internal IEnumerable<FormatterExtension> ParseExtensions(FormatterRequest request, out int index)
        {
            var result = new List<FormatterExtension>();
            int length = request.FormatterArguments.Length;
            index = 0;

            var extension = new StringBuilder();
            var value = new StringBuilder();

            const char colon = ':';
            bool foundExtension = false;
            for (int i = 0; i < length; i++)
            {
                var c = request.FormatterArguments[i];

                // Whitespace is tolerated at the beginning.
                bool isWhiteSpace = char.IsWhiteSpace(c);
                if(isWhiteSpace)
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

                if (c == colon)
                {
                    foundExtension = true;
                    continue;
                }

                if(foundExtension)
                {
                    value.Append(c);
                    continue;
                }
                
                extension.Append(c);

            }
            return result;
        }

        /// <summary>
        /// Parses the keyed blocks.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        protected internal IEnumerable<KeyedBlock> ParseKeyedBlocks(FormatterRequest request, int startIndex)
        {
            const char openBrace = '{';
            const char closeBrace = '}';
            const char escapingChar = '\\';

            var result = new List<KeyedBlock>();
            var key = new StringBuilder();
            var block = new StringBuilder();
            var braceBalance = 0;
            var foundWhitespaceAfterKey = false;
            for (int i = startIndex; i < request.FormatterArguments.Length; i++)
            {
                var c = request.FormatterArguments[i];
                var isWhitespace = char.IsWhiteSpace(c);

                if(c == openBrace)
                {
                    if(key.Length == 0)
                        throw new MalformedLiteralException("Expected a key, but found start of a new block.", 0, 0, request.FormatterArguments);

                    if(i != 0 && request.FormatterArguments[i-1] == escapingChar)
                    {
                        block.Append(c);
                        continue;
                    }
                    braceBalance++;
                    if (braceBalance > 1)
                        block.Append(c);
                    continue;
                }
                if(c == closeBrace)
                {
                    if (key.Length == 0)
                        throw new MalformedLiteralException("Expected a key, but found end of a block.", 0, 0, request.FormatterArguments);

                    if (i != 0 && request.FormatterArguments[i - 1] == escapingChar)
                    {
                        block.Append(c);
                        continue;
                    }
                    if(braceBalance == 0)
                    {
                        throw new MalformedLiteralException("Found end of a block, but no block has been started, or the" +
                                                            " block has already been closed. " +
                                                            "This could indicate an unescaped brace somewhere.", 
                            0, 0, request.FormatterArguments);
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
                    if (braceBalance < 0)
                    {
                        throw new MalformedLiteralException(
                            "Expected '{', but found '}' - essentially this means there are more close braces than there are open braces.",
                            0, 0, request.FormatterArguments);
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
                        throw new MalformedLiteralException(
                            "Any whitespace after a key should be followed by the beginning of a block.", 0, 0,
                            request.FormatterArguments);
                    key.Append(c);
                }
                else if(key.Length > 0) foundWhitespaceAfterKey = true;
            }
            if(braceBalance > 0)
            {
                throw new MalformedLiteralException(
                    "There are more open braces than there are close braces.",
                    0, 0, request.FormatterArguments);
            }
            return result;
        }

        /// <summary>
        /// Asserts the variable exists.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="variableName">Name of the variable.</param>
        protected internal void AssertVariableExists(Dictionary<string, object> args, string variableName)
        {
            if(args.ContainsKey(variableName) == false)
                throw new VariableNotFoundException(variableName);
        }
    }
}