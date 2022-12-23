// MessageFormat for .NET
// - MessageFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat
{
    /// <summary>
    ///     The magical Message Formatter.
    /// </summary>
    public class MessageFormatter : IMessageFormatter
    {
        #region Static Fields

        /// <summary>
        ///     The instance of MessageFormatter, with the default locale + cache settings.
        /// </summary>
        private static readonly IMessageFormatter Instance = new MessageFormatter();

        /// <summary>
        ///     The lock object.
        /// </summary>
        private static readonly object Lock = new object();

        #endregion

        #region Fields

        /// <summary>
        ///     Pattern cache. If enabled, should speed up formatting the same pattern multiple times,
        ///     regardless of arguments.
        /// </summary>
        private readonly ConcurrentDictionary<string, IFormatterRequestCollection>? cache;

        /// <summary>
        ///     The formatter library.
        /// </summary>
        private readonly IFormatterLibrary library;

        /// <summary>
        ///     The pattern parser
        /// </summary>
        private readonly IPatternParser patternParser;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageFormatter" /> class.
        /// </summary>
        /// <param name="useCache">
        ///     The use Cache.
        /// </param>
        /// <param name="locale">
        ///     The locale.
        /// </param>
        public MessageFormatter(bool useCache = true, string locale = "en")
            : this(new PatternParser(new LiteralParser()), new FormatterLibrary(), useCache, locale)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageFormatter" /> class.
        /// </summary>
        /// <param name="patternParser">
        ///     The pattern parser.
        /// </param>
        /// <param name="library">
        ///     The library.
        /// </param>
        /// <param name="useCache">
        ///     if set to <c>true</c> uses the cache.
        /// </param>
        /// <param name="locale">
        ///     The locale to use. Formatters may need this.
        /// </param>
        internal MessageFormatter(
            IPatternParser patternParser,
            IFormatterLibrary library,
            bool useCache,
            string locale = "en")
        {
            this.patternParser = patternParser ?? throw new ArgumentNullException("patternParser");
            this.library = library ?? throw new ArgumentNullException("library");
            this.Locale = locale;
            if (useCache)
            {
                this.cache = new ConcurrentDictionary<string, IFormatterRequestCollection>();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the formatters library, where you can add your own formatters if you want.
        /// </summary>
        /// <value>
        ///     The formatters.
        /// </value>
        public IFormatterLibrary Formatters
        {
            get { return this.library; }
        }

        /// <summary>
        ///     Gets or sets the locale.
        /// </summary>
        /// <value>
        ///     The locale.
        /// </value>
        public string Locale { get; set; }

        /// <summary>
        ///     Gets the pluralizers dictionary from the <see cref="PluralFormatter" />, if set. Key is the locale.
        /// </summary>
        /// <value>
        ///     The pluralizers, or <c>null</c> if the plural formatter has not been added.
        /// </value>
        public IDictionary<string, Pluralizer>? Pluralizers
        {
            get
            {
                var pluralFormatter = this.Formatters.OfType<PluralFormatter>().FirstOrDefault();
                return pluralFormatter?.Pluralizers;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Formats the specified pattern with the specified data.
        /// </summary>
        /// <remarks>
        ///     This method calls <see cref="Format(string, IReadOnlyDictionary{string, object})"/>
        ///     on a singleton instance using a lock.
        ///     Do not use in a tight loop, as a lock is being used to ensure thread safety.
        /// </remarks>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <returns>
        ///     The formatted message.
        /// </returns>
        public static string Format(string pattern, IReadOnlyDictionary<string, object?> data)
        {
            lock (Lock)
            {
                return Instance.FormatMessage(pattern, data);
            }
        }

        /// <summary>
        ///     Formats the specified pattern with the specified data.
        /// </summary>
        /// This method calls
        /// <see cref="FormatMessage(string, object)" />
        /// on a singleton instance using a lock.
        /// Do not use in a tight loop, as a lock is being used to ensure thread safety.
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <returns>
        ///     The formatted message.
        /// </returns>
        public static string Format(string pattern, object data)
        {
            lock (Lock)
            {
                return Instance.FormatMessage(pattern, data);
            }
        }

        /// <summary>
        ///     Formats the message with the specified arguments. It's so magical.
        /// </summary>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string FormatMessage(string pattern, IReadOnlyDictionary<string, object?> args)
        {
            /*
             * We are assuming the formatters are ordered correctly
             * - that is, from left to right, string-wise.
             */
            var sourceBuilder = StringBuilderPool.Get();

            try
            {
                sourceBuilder.Append(pattern);
                var requests = this.ParseRequests(pattern, sourceBuilder);

                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];

                    var formatter = this.Formatters.GetFormatter(request);

                    if (args.TryGetValue(request.Variable, out var value) == false && formatter.VariableMustExist)
                    {
                        throw new VariableNotFoundException(request.Variable);
                    }

                    // Double dispatch, yeah!
                    var result = formatter.Format(this.Locale, request, args, value, this);

                    // First, we remove the literal from the source.
                    var sourceLiteral = request.SourceLiteral;

                    // +1 because we want to include the last index.
                    var length = (sourceLiteral.EndIndex - sourceLiteral.StartIndex) + 1;
                    sourceBuilder.Remove(sourceLiteral.StartIndex, length);

                    // Now, we inject the result.
                    sourceBuilder.Insert(sourceLiteral.StartIndex, result);

                    // The next requests will want to know what happened.
                    requests.ShiftIndices(i, result.Length);
                }

                // And we're done.
                return MessageFormatter.UnescapeLiterals(sourceBuilder);
            }
            finally
            {
                StringBuilderPool.Return(sourceBuilder);
            }
        }

        /// <summary>
        ///     Formats the message, and uses reflection to create a dictionary of property values from the specified object.
        /// </summary>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string FormatMessage(string pattern, object args)
        {
            return this.FormatMessage(pattern, args.ToDictionary());
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Unescapes the literals from the source builder, and returns a new instance with literals unescaped.
        /// </summary>
        /// <param name="sourceBuilder">
        ///     The source builder.
        /// </param>
        /// <returns>
        ///     The <see cref="StringBuilder" />.
        /// </returns>
        internal static string UnescapeLiterals(StringBuilder sourceBuilder)
        {
            // If the block is empty, do nothing.
            if (sourceBuilder.Length == 0)
            {
                return string.Empty;
            }

            const char EscapingChar = '\'';

            if (!sourceBuilder.Contains(EscapingChar))
            {
                return sourceBuilder.ToString();
            }

            var length = sourceBuilder.Length;
            var insideEscapeSequence = false;

            var dest = StringBuilderPool.Get();

            try
            {
                for (int i = 0; i < length; i++)
                {
                    var c = sourceBuilder[i];

                    if (c == EscapingChar)
                    {
                        if (i == length - 1)
                        {
                            if (!insideEscapeSequence)
                                dest.Append(EscapingChar);
                            continue;
                        }

                        var nextChar = sourceBuilder[i + 1];
                        if (nextChar == EscapingChar)
                        {
                            dest.Append(EscapingChar);
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
                            dest.Append(nextChar);
                            insideEscapeSequence = true;
                            ++i;
                            continue;
                        }

                        dest.Append(EscapingChar);
                        continue;
                    }

                    dest.Append(c);
                }

                return dest.ToString();
            }
            finally
            {
                StringBuilderPool.Return(dest);
            }
        }

        /// <summary>
        ///     Parses the requests, using the cache if enabled and applicable.
        /// </summary>
        /// <param name="pattern">
        ///     The pattern.
        /// </param>
        /// <param name="sourceBuilder">
        ///     The source builder.
        /// </param>
        /// <returns>
        ///     The <see cref="IFormatterRequestCollection" />.
        /// </returns>
        private IFormatterRequestCollection ParseRequests(string pattern, StringBuilder sourceBuilder)
        {
            // If we are not using the cache, just parse them straight away.
            if (this.cache == null)
            {
                return this.patternParser.Parse(sourceBuilder);
            }

            // If we have a cached result from this pattern, clone it and return the clone.
            if (this.cache.TryGetValue(pattern, out var cached))
            {
                return cached.Clone();
            }

            var requests = this.patternParser.Parse(sourceBuilder);
            this.cache?.TryAdd(pattern, requests.Clone());

            return requests;
        }

        #endregion
    }
}