using System.Text;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

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
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");

            WriteLine("namespace Jeffijoe.MessageFormat.Formatting.Formatters");
            WriteLine("{");
            AddIndent();

            WriteLine("public static partial class PluralRulesMetadata");
            WriteLine("{");
            AddIndent();

            for (var ruleIdx = 0; ruleIdx < _rules.Length; ruleIdx++)
            {
                var rule = _rules[ruleIdx];

                var ruleGenerator = new RuleGenerator(rule);

                foreach(var locale in rule.Locales)
                {
                    WriteLine($"public static string Locale_{locale.ToUpper()}(double value) => Rule{ruleIdx}(value);");
                    WriteLine(string.Empty);
                }

                WriteLine($"private static string Rule{ruleIdx}(double value)");
                WriteLine("{");
                AddIndent();

                ruleGenerator.WriteTo(_sb, _indent);

                DecreaseIndent();
                WriteLine("}");
                WriteLine(string.Empty);
            }

            WriteLine("private static readonly Dictionary<string, Pluralizer> Pluralizers = new Dictionary<string, Pluralizer>()");
            WriteLine("{");
            AddIndent();

            for (int ruleIdx = 0; ruleIdx < _rules.Length; ruleIdx++)
            {
                PluralRule rule = _rules[ruleIdx];
                foreach (var locale in rule.Locales)
                {
                    WriteLine($"{{\"{locale}\", (Pluralizer) Rule{ruleIdx}}},");
                }

                WriteLine(string.Empty);
            }

            DecreaseIndent();
            WriteLine("};");
            WriteLine(string.Empty);

            WriteLine("public static partial bool TryGetRuleByLocale(string locale, out Pluralizer pluralizer)");
            WriteLine("{");
            AddIndent();

            WriteLine("return Pluralizers.TryGetValue(locale, out pluralizer);");

            DecreaseIndent();
            WriteLine("}");

            DecreaseIndent();
            WriteLine("}");

            DecreaseIndent();
            WriteLine("}");

            return _sb.ToString();
        }

        private void AddIndent() => _indent += 4;
        private void DecreaseIndent() => _indent -= 4;

        private void WriteLine(string line)
        {
            _sb.Append(' ', _indent);
            _sb.AppendLine(line);
        }
    }
}
