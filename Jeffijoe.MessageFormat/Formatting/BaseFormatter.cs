using System;
using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Helpers;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Base formatter with helpers for extracting data from the formatter request.
    /// </summary>
    public abstract class BaseFormatter
    {
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

                if(c.IsAlphaNumeric() == false)
                {
                    throw new MessageFormatterException("Extension names can only be alpha-numerical characters. Not even sure numerical characters are valid, but what the hell.");
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
            var openBraces = 0;

            for (int i = startIndex; i < request.FormatterArguments.Length; i++)
            {
                var c = request.FormatterArguments[i];
                var isWhitespace = char.IsWhiteSpace(c);

                if(c == openBrace)
                {
                    if(i != 0 && request.FormatterArguments[i-1] == escapingChar) continue;
                    openBraces++;
                    continue;
                }
                if(c == closeBrace)
                {
                    if (i != 0 && request.FormatterArguments[i - 1] == escapingChar) continue;
                    openBraces--;
                    if (openBraces == 0)
                    {
                        result.Add(new KeyedBlock(key.ToString(), block.ToString()));
                        block.Clear();
                        key.Clear();
                        continue;
                    }
                }

                // If we are inside a block, append to the block buffer
                if (openBraces > 0)
                {
                    block.Append(c);
                    continue;
                }

                // Else, we are buffering our key
                if(isWhitespace == false)
                {
                    key.Append(c);
                }
            }
            return result;
        }
    }
}