using System.Text;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

public class PluralRulesMetadataGenerator
{
    private readonly PluralRuleSet _rules;
    private readonly StringBuilder _sb;
    private int _indent;

    public PluralRulesMetadataGenerator(PluralRuleSet rules)
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

        WriteLine("internal static partial class PluralRulesMetadata");
        WriteLine("{");
        AddIndent();

        // Generate a method for each unique rule, by index, that chooses the plural form
        // for a given input source number (the PluralContext) according to that rule.
        var uniqueRules = _rules.UniqueRules;
        for (var ruleIdx = 0; ruleIdx < uniqueRules.Count; ruleIdx++)
        {
            var rule = uniqueRules[ruleIdx];
            var ruleGenerator = new RuleGenerator(rule);

            WriteLine($"private static string Rule{ruleIdx}(PluralContext context)");
            WriteLine("{");
            AddIndent();

            ruleGenerator.WriteTo(_sb, _indent);

            DecreaseIndent();
            WriteLine("}");
            WriteLine(string.Empty);
        }

        // Generate a static lookup dictionary of each (locale, plural type) to the corresponding rule method
        // to use for that locale and type.
        WriteLine("private static readonly Dictionary<PluralLookupKey, ContextPluralizer> Pluralizers = new Dictionary<PluralLookupKey, ContextPluralizer>()");
        WriteLine("{");
        AddIndent();

        foreach (var kvp in _rules.RuleIndicesByKey)
        {
            string locale = kvp.Key.Locale;
            string pluralType = kvp.Key.PluralType;
            int ruleIdx = kvp.Value;
            
            WriteLine($"{{new PluralLookupKey(Locale: \"{locale}\", PluralType: \"{pluralType}\"), Rule{ruleIdx}}},");
            WriteLine(string.Empty);
        }

        DecreaseIndent();
        WriteLine("};");
        WriteLine(string.Empty);

        // Finally generate our public API to the rest of the library, that takes a locale and pluralType
        // and tries to retrieve an appropriate localizer to map an input source number to the form for the request.
        WriteLine("public static partial bool TryGetRuleByLocale(PluralLookupKey key, out ContextPluralizer contextPluralizer)");
        WriteLine("{");
        AddIndent();

        WriteLine("return Pluralizers.TryGetValue(key, out contextPluralizer);");

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