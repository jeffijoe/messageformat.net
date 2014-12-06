using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat
{
    /// <summary>
    /// The magical Message Formatter.
    /// </summary>
    public class MessageFormatter : IMessageFormatter
    {
        /// <summary>
        /// The pattern parser
        /// </summary>
        private readonly IPatternParser _patternParser;

        /// <summary>
        /// The formatter library.
        /// </summary>
        private readonly IFormatterLibrary _library;

        /// <summary>
        /// Pattern cache. If enabled, should speed up formatting the same pattern multiple times,
        /// regardless of arguments.
        /// </summary>
        private readonly Dictionary<string, IFormatterRequestCollection> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatter"/> class.
        /// </summary>
        public MessageFormatter(bool useCache = true, string locale = "en")
            : this(new PatternParser(new LiteralParser()), new FormatterLibrary(), useCache, locale)
        {
            
        }

        /// <summary>
        /// Gets the pluralizers dictionary from the <see cref="PluralFormatter"/>, if set. Key is the locale.
        /// </summary>
        /// <value>
        /// The pluralizers, or <c>null</c> if the plural formatter has not been added.
        /// </value>
        public Dictionary<string, Pluralizer> Pluralizers
        {
            get
            {
                var pluralFormatter = Formatters.OfType<PluralFormatter>().FirstOrDefault();
                if (pluralFormatter == null) return null;
                return pluralFormatter.Pluralizers;
            }
        }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>
        /// The locale.
        /// </value>
        public string Locale { get; set; }

        /// <summary>
        /// Gets the formatters library, where you can add your own formatters if you want.
        /// </summary>
        /// <value>
        /// The formatters.
        /// </value>
        public IFormatterLibrary Formatters
        {
            get { return _library; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatter" /> class.
        /// </summary>
        /// <param name="patternParser">The pattern parser.</param>
        /// <param name="library">The library.</param>
        /// <param name="useCache">if set to <c>true</c> uses the cache.</param>
        /// <param name="locale">The locale to use. Formatters may need this.</param>
        /// <exception cref="System.ArgumentNullException">patternParser</exception>
        internal MessageFormatter(IPatternParser patternParser, IFormatterLibrary library, bool useCache, string locale = "en")
        {
            if (patternParser == null) throw new ArgumentNullException("patternParser");
            if (library == null) throw new ArgumentNullException("library");
            _patternParser = patternParser;
            _library = library;
            Locale = locale;
            if (useCache)
                cache = new Dictionary<string, IFormatterRequestCollection>();
        }

        /// <summary>
        /// Formats the message with the specified arguments. It's so magical.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public string FormatMessage(string pattern, Dictionary<string, object> args)
        {
            /*
             * We are asuming the formatters are ordered correctly
             * - that is, from left to right, string-wise.
             */

            var sourceBuilder = new StringBuilder(pattern);
            var requests = ParseRequests(pattern, sourceBuilder);
            var requestsEnumerated = requests.ToArray();

            // If we got no formatters, then we're done here.
            if (requestsEnumerated.Length == 0) return pattern;

            for (int i = 0; i < requestsEnumerated.Length; i++)
            {
                var request = requestsEnumerated[i];
                var formatter = Formatters.GetFormatter(request);
                if (formatter == null)
                    throw new FormatterNotFoundException(request);

                // Double dispatch, yeah!
                var result = formatter.Format(Locale, request, args, this);

                // First, we remove the literal from the source.
                Literal sourceLiteral = request.SourceLiteral;
                // +1 because we want to include the last index.
                var length = (sourceLiteral.EndIndex - sourceLiteral.StartIndex) + 1;
                sourceBuilder.Remove(sourceLiteral.StartIndex, length);

                // Now, we inject the result.
                sourceBuilder.Insert(sourceLiteral.StartIndex, result);

                // The next requests will want to know what happened.
                requests.ShiftIndices(i, result.Length);
            }

            sourceBuilder = UnescapeLiterals(sourceBuilder);

            // And we're done.
            return sourceBuilder.ToString();
        }

        /// <summary>
        /// Parses the requests, using the cache if enabled and applicable.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="sourceBuilder">The source builder.</param>
        /// <returns></returns>
        private IFormatterRequestCollection ParseRequests(string pattern, StringBuilder sourceBuilder)
        {
            // If we are not using the cache, just parse them straight away.
            if (cache == null)
                return _patternParser.Parse(sourceBuilder);

            // If we have a cached result from this pattern, clone it and return the clone.
            IFormatterRequestCollection cached;
            if (cache.TryGetValue(pattern, out cached))
            {
                return cached.Clone();
            }
            var requests = _patternParser.Parse(sourceBuilder);
            if (cache != null)
                cache.Add(pattern, requests.Clone());
            return requests;
        }

        /// <summary>
        /// Formats the message, and uses reflection to create a dictionary of property values from the specified object.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public string FormatMessage(string pattern, object args)
        {
            return FormatMessage(pattern, args.ToDictionary());
        }

        /// <summary>
        /// Unescapes the literals from the source builder, and returns a new instance with literals unescaped.
        /// </summary>
        /// <param name="sourceBuilder">The source builder.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected internal StringBuilder UnescapeLiterals(StringBuilder sourceBuilder)
        {
            var dest = new StringBuilder(sourceBuilder.Length, sourceBuilder.Length);
            int length = sourceBuilder.Length;
            const char escapeChar = '\\';
            const char openBrace = '{';
            const char closeBrace = '}';
            var braceBalance = 0;
            for (int i = 0; i < length; i++)
            {
                var c = sourceBuilder[i];
                if (c == escapeChar)
                {
                    if (i != length - 1)
                    {
                        char next = sourceBuilder[i + 1];
                        if (next == openBrace && braceBalance == 0)
                        {
                            continue;
                        }

                        if (next == closeBrace && braceBalance == 1)
                        {
                            continue;
                        }
                    }
                }
                else if (c == openBrace)
                {
                    braceBalance++;
                }
                else if (c == closeBrace)
                {
                    braceBalance--;
                }
                dest.Append(c);
            }
            return dest;
        }
    }
}