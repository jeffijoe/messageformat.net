using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat
{
    /// <summary>
    /// The magical Message Formatter.
    /// </summary>
    public class MessageFormatter : IMessageFormatter
    {
        private readonly IPatternParser _patternParser;
        private readonly IFormatterLibrary _library;
        private PluralFormatter _pluralFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatter"/> class.
        /// </summary>
        public MessageFormatter(string locale = "en")
            : this(new PatternParser(new LiteralParser()), new FormatterLibrary(), locale)
        {
            Formatters.Add(new VariableFormatter());
            Formatters.Add(new SelectFormatter());
            _pluralFormatter = new PluralFormatter();
            Formatters.Add(_pluralFormatter);
        }

        /// <summary>
        /// Gets the pluralizers dictionary. Key is the locale.
        /// </summary>
        /// <value>
        /// The pluralizers.
        /// </value>
        public Dictionary<string, Pluralizer> Pluralizers
        {
            get { return _pluralFormatter.Pluralizers; }
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
        /// <param name="library"></param>
        /// <param name="locale">The locale to use. Formatters may need this.</param>
        /// <exception cref="System.ArgumentNullException">patternParser</exception>
        internal MessageFormatter(IPatternParser patternParser, IFormatterLibrary library, string locale = "en")
        {
            if (patternParser == null) throw new ArgumentNullException("patternParser");
            if (library == null) throw new ArgumentNullException("library");
            _patternParser = patternParser;
            _library = library;
            Locale = locale;
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
            var requests = _patternParser.Parse(sourceBuilder);
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
        /// Unescapes the literals from the source builder, and returns a new instance with literals unescaped..
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
            int braceBalance = 0;
            for (int i = 0; i < length; i++)
            {
                var c = sourceBuilder[i];
                if(c == escapeChar)
                {
                    if (i != length - 1)
                    {
                        char next = sourceBuilder[i+1];
                        if (next == openBrace)
                        {
                            braceBalance++;
                            continue;
                        }

                        if(next == closeBrace)
                        {
                            braceBalance--;
                            continue;
                        }
                    }
                }
                dest.Append(c);
            }
            return dest;
        }
    }
}