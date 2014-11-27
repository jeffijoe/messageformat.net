using System;
using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Parsers.Literals;

namespace Jeffijoe.MessageFormat.Parsers.Patterns
{
    /// <summary>
    /// Pattern parser.
    /// </summary>
    public class PatternParser : IPatternParser
    {
        private readonly ILiteralParser _literalParser;

        public PatternParser(ILiteralParser literalParser)
        {
            if (literalParser == null) throw new ArgumentNullException("literalParser");
            _literalParser = literalParser;
        }

        public void Parse(StringBuilder source)
        {
            var literals = _literalParser.ParseLiterals(source).ToList();
            foreach (var literal in literals)
            {
                // The first token to follow an opening brace will be the key, ended by a comma.
                var key = GetKey(literal);
                
            }
        }

        /// <summary>
        /// Gets the key from the literal.
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns></returns>
        /// <exception cref="Jeffijoe.MessageFormat.Parsers.MalformedLiteralException">
        /// Parsing the variable key yielded an empty string.
        /// </exception>
        internal static StringBuilder GetKey(Literal literal)
        {
            const char comma = ',';
            var sb = new StringBuilder();
            var innerText = literal.InnerText;
            var column = literal.SourceColumnNumber;
            for (var i = 0; i < innerText.Length; i++)
            {
                var c = innerText[i];
                column++;
                if (c == comma) break;
                if (c.IsAlphaNumeric() == false)
                {
                    var msg = string.Format("Invalid literal character '{0}'.", c);
                    // Line number can't have changed.
                    throw new MalformedLiteralException(msg, literal.SourceLineNumber, column, innerText.ToString());
                }
                sb.Append(c);
            }
            if(sb.Length == 0)
                throw new MalformedLiteralException("Parsing the literal yielded an empty string.", literal.SourceLineNumber, column);
            return sb;
        }
    }
}