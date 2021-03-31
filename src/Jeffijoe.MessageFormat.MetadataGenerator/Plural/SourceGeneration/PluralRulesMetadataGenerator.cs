using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

using System.Text;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration
{
    public class PluralRulesMetadataGenerator
    {
        private readonly PluralRule[] _rules;
        private readonly StringBuilder _sb;
        private int _indent;

        public PluralRulesMetadataGenerator(PluralRule[] rules)
        {
            _rules = rules;
            _sb = new StringBuilder();
        }

        public string GenerateClass()
        {
            Write("using System;");
            Write("using System.Collections.Generic;");

            Write("namespace Jeffijoe.MessageFormat.Formatting.Formatters");
            Write("{");
            _indent += 4;

            Write("public static partial class PluralRulesMetadata");
            Write("{");
            _indent += 4;

            for(var ruleIdx = 0; ruleIdx < _rules.Length; ruleIdx++)
            {
                var rule = _rules[ruleIdx];

                var ruleGenerator = new RuleGenerator(rule);

                foreach(var locale in rule.Locales)
                {
                    Write($"private static string Locale_{locale.ToUpper()}(double value)");
                    Write("{");
                    _indent += 4;

                    Write($"return Rule{ruleIdx}(value);");

                    _indent -= 4;
                    Write("}");
                }

                Write($"private static string Rule{ruleIdx}(double value)");
                Write("{");
                _indent += 4;
                ruleGenerator.WriteTo(_sb, _indent);
                _indent -= 4;
                Write("}");
            }

            Write("public static partial void AddAllRules(IDictionary<string, Pluralizer> pluralizers)");
            Write("{");
            _indent += 4;

            for (int ruleIdx = 0; ruleIdx < _rules.Length; ruleIdx++)
            {
                PluralRule rule = _rules[ruleIdx];
                foreach (var locale in rule.Locales)
                {
                    Write($"pluralizers.Add(\"{locale}\", (Pluralizer) Rule{ruleIdx});");
                }

                Write(string.Empty);
            }

            _indent -= 4;
            Write("}");

            _indent -= 4;
            Write("}");

            _indent -= 4;
            Write("}");

            return _sb.ToString();
        }

        private void Write(string line)
        {
            _sb.Append(' ', _indent);
            _sb.AppendLine(line);
        }
    }
}
