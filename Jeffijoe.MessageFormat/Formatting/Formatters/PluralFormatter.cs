// PluralFormatter.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    /// Plural Formatter
    /// </summary>
    public class PluralFormatter : BaseFormatter, IFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluralFormatter"/> class.
        /// </summary>
        public PluralFormatter()
        {
            Pluralizers = new Dictionary<string, Pluralizer>();
            AddStandardPluralizers();
        }

        /// <summary>
        /// Adds the standard pluralizers.
        /// </summary>
        private void AddStandardPluralizers()
        {
            Pluralizers.Add("en", n =>
            {
                if (Math.Abs(n) < Double.Epsilon)
                    return "zero";
                if (Math.Abs(n - 1) < Double.Epsilon)
                    return "one";
                return "other";
            });
        }

        /// <summary>
        /// Gets the pluralizers dictionary. Key is the locale.
        /// </summary>
        /// <value>
        /// The pluralizers.
        /// </value>
        public Dictionary<string, Pluralizer> Pluralizers { get; private set; }

        /// <summary>
        /// Determines whether this instance can format a message based on the specified parameters.
        /// </summary>
        /// <param name="request">The parameters.</param>
        /// <returns></returns>
        public bool CanFormat(FormatterRequest request)
        {
            return request.FormatterName == "plural";
        }

        /// <summary>
        /// Using the specified parameters and arguments, a formatted string shall be returned.
        /// The <see cref="IMessageFormatter" /> is being provided as well, to enable
        /// nested formatting. This is only called if <see cref="CanFormat" /> returns true.
        /// The <see cref="args" /> will always contain the <see cref="FormatterRequest.Variable" />.
        /// </summary>
        /// <param name="locale">The locale being used. It is up to the formatter what they do with this information.</param>
        /// <param name="request">The parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="messageFormatter">The message formatter.</param>
        /// <returns></returns>
        public string Format(string locale, FormatterRequest request, Dictionary<string, object> args, IMessageFormatter messageFormatter)
        {
            var arguments = ParseArguments(request);
            double offset = 0;
            var offsetExtension = arguments.Extensions.FirstOrDefault(x => x.Extension == "offset");
            if (offsetExtension != null)
                offset = Convert.ToDouble(offsetExtension.Value);

            double n = 0;
            object varResult = null;
            if(args.TryGetValue(request.Variable, out varResult))
            {
                n = Convert.ToDouble(varResult);
            }
            
            var pluralized = new StringBuilder(Pluralize(locale, request, arguments, args, n, offset));
            var result =  ReplaceNumberLiterals(pluralized, n - offset);
            var formatted = messageFormatter.FormatMessage(result, args);
            return formatted;
        }

        /// <summary>
        /// Replaces the number literals with the actual number.
        /// </summary>
        /// <param name="pluralized">The pluralized.</param>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        internal string ReplaceNumberLiterals(StringBuilder pluralized, double n)
        {
            // I've done this a few times now..
            const char openBrace = '{';
            const char closeBrace = '}';
            const char pound = '#';
            const char escapeChar = '\\';
            var braceBalance = 0;
            var sb = new StringBuilder();
            for (int i = 0; i < pluralized.Length; i++)
            {
                var c = pluralized[i];

                if(c == openBrace)
                {
                    if(i != 0)
                        if (pluralized[i - 1] != escapeChar)
                            braceBalance++;
                }
                else if(c == closeBrace)
                {
                    if(i != 0)
                        if (pluralized[i - 1] != escapeChar)
                            braceBalance--;
                }
                else if(c == pound)
                {
                    if (i != 0)
                    {
                        if (pluralized[i - 1] != escapeChar)
                        {
                            if (braceBalance == 0)
                            {
                                sb.Append(n);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        sb.Append(n);
                        continue;
                    } 
                        
                }
                else if(c == escapeChar)
                {
                    if(i != pluralized.Length)
                        if(pluralized[i+1] == pound)
                            continue; // Next one is an escaped number literal, so we don't want the escaping char.
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the correct plural block.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="request">The request.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="n"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <exception cref="MessageFormatterException">'other' option not found in pattern.</exception>
        internal string Pluralize(string locale, FormatterRequest request, ParsedArguments arguments, Dictionary<string, object> args, double n, double offset)
        {
            Pluralizer pluralizer;
            if (Pluralizers.TryGetValue(locale, out pluralizer) == false)
                pluralizer = Pluralizers["en"];
            var pluralForm = pluralizer(n - offset);
            KeyedBlock other = null;
            foreach (var keyedBlock in arguments.KeyedBlocks)
            {
                if (keyedBlock.Key == Other)
                    other = keyedBlock;

                if (keyedBlock.Key.StartsWith("="))
                {
                    var numberLiteral = Convert.ToDouble(keyedBlock.Key.Substring(1));
                    if (Math.Abs(numberLiteral - n) < Double.Epsilon)
                        return keyedBlock.BlockText;
                }
                if (keyedBlock.Key == pluralForm)
                {
                    return keyedBlock.BlockText;
                }
            }
            if (other == null)
                throw new MessageFormatterException("'other' option not found in pattern.");
            return other.BlockText;
        }
    }

    

    /// <summary>
    /// Given the specified number, determines what plural form is being used.
    /// </summary>
    /// <param name="n">The number used to determine the pluralization rule..</param>
    /// <returns></returns>
    public delegate string Pluralizer(double n);
}