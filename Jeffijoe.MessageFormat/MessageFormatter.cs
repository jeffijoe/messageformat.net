using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
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

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>
        /// The locale.
        /// </value>
        public string Locale { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatter" /> class.
        /// </summary>
        /// <param name="patternParser">The pattern parser.</param>
        /// <param name="library"></param>
        /// <param name="locale">The locale to use. Formatters may need this.</param>
        /// <exception cref="System.ArgumentNullException">patternParser</exception>
        public MessageFormatter(IPatternParser patternParser, IFormatterLibrary library, string locale = "en")
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
            
            ValidateVariableExistence(requestsEnumerated, args);

            for (int i = 0; i < requestsEnumerated.Length; i++)
            {
                var request = requestsEnumerated[i];
                var formatter = _library.GetFormatter(request);
                if (formatter == null)
                    throw new FormatterNotFoundException(request);

                // Double dispatch, yeah!
                var result = formatter.Format(Locale, request, args, this);

                // First, we remove the literal from the source.
                Literal sourceLiteral = request.SourceLiteral;
                sourceBuilder.Remove(sourceLiteral.StartIndex, (sourceLiteral.EndIndex - sourceLiteral.StartIndex) + 1);

                // Now, we inject the result.
                sourceBuilder.Insert(sourceLiteral.StartIndex, result);

                // The next requests will want to know what happened.
                requests.ShiftIndices(i, result.Length);
            }
            // And we're done.
            return sourceBuilder.ToString();
        }

        /// <summary>
        /// Validates that all variables exist in the args collection.
        /// </summary>
        /// <param name="formatters">The formatters.</param>
        /// <param name="args">The arguments.</param>
        internal static void ValidateVariableExistence(IEnumerable<FormatterRequest> formatters, Dictionary<string, object> args)
        {
            foreach (var formatterParameters in formatters)
            {
                if(args.ContainsKey(formatterParameters.Variable) == false)
                    throw new VariableNotFoundException(formatterParameters.Variable);
            }
        }
    }
}