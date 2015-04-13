// MessageFormat for .NET
// - PluralFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    ///     Plural Formatter
    /// </summary>
    public class PluralFormatter : BaseFormatter, IFormatter
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PluralFormatter" /> class.
        /// </summary>
        public PluralFormatter()
        {
            this.Pluralizers = new Dictionary<string, Pluralizer>();
            this.AddStandardPluralizers();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the pluralizers dictionary. Key is the locale.
        /// </summary>
        /// <value>
        ///     The pluralizers.
        /// </value>
        public Dictionary<string, Pluralizer> Pluralizers { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether this instance can format a message based on the specified parameters.
        /// </summary>
        /// <param name="request">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanFormat(FormatterRequest request)
        {
            return request.FormatterName == "plural";
        }

        /// <summary>
        ///     Using the specified parameters and arguments, a formatted string shall be returned.
        ///     The <see cref="IMessageFormatter" /> is being provided as well, to enable
        ///     nested formatting. This is only called if <see cref="CanFormat" /> returns true.
        ///     The <see cref="args" /> will always contain the <see cref="FormatterRequest.Variable" />.
        /// </summary>
        /// <param name="locale">
        ///     The locale being used. It is up to the formatter what they do with this information.
        /// </param>
        /// <param name="request">
        ///     The parameters.
        /// </param>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <param name="messageFormatter">
        ///     The message formatter.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string Format(
            string locale, 
            FormatterRequest request, 
            Dictionary<string, object> args, 
            IMessageFormatter messageFormatter)
        {
            var arguments = this.ParseArguments(request);
            double offset = 0;
            var offsetExtension = arguments.Extensions.FirstOrDefault(x => x.Extension == "offset");
            if (offsetExtension != null)
            {
                offset = Convert.ToDouble(offsetExtension.Value);
            }

            double n = 0;
            object varResult = null;
            if (args.TryGetValue(request.Variable, out varResult))
            {
                n = Convert.ToDouble(varResult);
            }

            var pluralized = new StringBuilder(this.Pluralize(locale, arguments, n, offset));
            var result = this.ReplaceNumberLiterals(pluralized, n - offset);
            var formatted = messageFormatter.FormatMessage(result, args);
            return formatted;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns the correct plural block.
        /// </summary>
        /// <param name="locale">
        ///     The locale.
        /// </param>
        /// <param name="arguments">
        ///     The parsed arguments string.
        /// </param>
        /// <param name="n">
        ///     The n.
        /// </param>
        /// <param name="offset">
        ///     The offset.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// <exception cref="MessageFormatterException">
        ///     The 'other' option was not found in pattern.
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", 
            Justification = "Reviewed. Suppression is OK here.")]
        internal string Pluralize(string locale, ParsedArguments arguments, double n, double offset)
        {
            Pluralizer pluralizer;
            if (this.Pluralizers.TryGetValue(locale, out pluralizer) == false)
            {
                pluralizer = this.Pluralizers["en"];
            }

            var pluralForm = pluralizer(n - offset);
            KeyedBlock other = null;
            foreach (var keyedBlock in arguments.KeyedBlocks)
            {
                if (keyedBlock.Key == OtherKey)
                {
                    other = keyedBlock;
                }

                if (keyedBlock.Key.StartsWith("="))
                {
                    var numberLiteral = Convert.ToDouble(keyedBlock.Key.Substring(1));
                    if (Math.Abs(numberLiteral - n) < double.Epsilon)
                    {
                        return keyedBlock.BlockText;
                    }
                }

                if (keyedBlock.Key == pluralForm)
                {
                    return keyedBlock.BlockText;
                }
            }

            if (other == null)
            {
                throw new MessageFormatterException("'other' option not found in pattern.");
            }

            return other.BlockText;
        }

        /// <summary>
        ///     Replaces the number literals with the actual number.
        /// </summary>
        /// <param name="pluralized">
        ///     The pluralized.
        /// </param>
        /// <param name="n">
        ///     The n.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        internal string ReplaceNumberLiterals(StringBuilder pluralized, double n)
        {
            // I've done this a few times now..
            const char OpenBrace = '{';
            const char CloseBrace = '}';
            const char Pound = '#';
            const char EscapeChar = '\\';
            var braceBalance = 0;
            var sb = new StringBuilder();
            for (int i = 0; i < pluralized.Length; i++)
            {
                var c = pluralized[i];

                if (c == OpenBrace)
                {
                    if (i != 0)
                    {
                        if (pluralized[i - 1] != EscapeChar)
                        {
                            braceBalance++;
                        }
                    }
                }
                else if (c == CloseBrace)
                {
                    if (i != 0)
                    {
                        if (pluralized[i - 1] != EscapeChar)
                        {
                            braceBalance--;
                        }
                    }
                }
                else if (c == Pound)
                {
                    if (i != 0)
                    {
                        if (pluralized[i - 1] != EscapeChar)
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
                else if (c == EscapeChar)
                {
                    if (i != pluralized.Length)
                    {
                        if (pluralized[i + 1] == Pound)
                        {
                            continue; // Next one is an escaped number literal, so we don't want the escaping char.
                        }
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Adds the standard pluralizers.
        /// </summary>
        private void AddStandardPluralizers()
        {
            this.Pluralizers.Add(
                "en", 
                n => {
                    if (Math.Abs(n) < double.Epsilon)
                    {
                        return "zero";
                    }

                    if (Math.Abs(n - 1) < double.Epsilon)
                    {
                        return "one";
                    }

                    return "other";
                });
        }

        #endregion
    }
}