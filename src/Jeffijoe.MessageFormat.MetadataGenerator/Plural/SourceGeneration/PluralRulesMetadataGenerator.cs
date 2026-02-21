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
        WriteLine("#nullable enable");
        WriteLine("using System;");
        WriteLine("using System.Collections.Generic;");
        WriteLine("using System.Diagnostics.CodeAnalysis;");

        WriteLine("namespace Jeffijoe.MessageFormat.Formatting.Formatters");
        WriteLine("{");
        AddIndent();

        WriteLine("internal static partial class PluralRulesMetadata");
        WriteLine("{");
        AddIndent();

        // Export a constant for the normalized root locale to match the logic we're using internally.
        // This way the rest of the lib's locale chaining can continue to work if we swap out
        // normalization internally.
        WriteLine($"public static readonly string RootLocale = \"{PluralRuleSet.RootLocale}\";");

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

        // Generate a static lookup dictionary of locale (case-insensitive) to LocalePluralizers for that locale.
        // e.g.,
        // en -> {
        //     Cardinal = Rule0,
        //     Ordinal = Rule1,
        // },
        // [etc for other locales, with some null values for unmapped locales]
        WriteLine("private static readonly Dictionary<string, LocalePluralizers> Pluralizers = new(StringComparer.OrdinalIgnoreCase)");
        WriteLine("{");
        AddIndent();

        foreach (var kvp in _rules.RuleIndicesByLocale)
        {
            string locale = kvp.Key;

            // When index is defined, we want "Rule#" as a reference to the delegate generated above;
            // otherwise we want null.
            int? cardinalIdx = kvp.Value.CardinalRuleIndex;
            int? ordinalIdx = kvp.Value.OrdinalRuleIndex;
            string cardinalValue = cardinalIdx is not null ? $"Rule{cardinalIdx}" : "null";
            string ordinalValue = ordinalIdx is not null ? $"Rule{ordinalIdx}" : "null";

            WriteLine($"{{\"{locale}\", new LocalePluralizers(Cardinal: {cardinalValue}, Ordinal: {ordinalValue})}},");
        }

        DecreaseIndent();
        WriteLine("};");
        WriteLine(string.Empty);

        // Finally generate our public API to the rest of the library, that takes a locale and pluralType
        // and tries to retrieve an appropriate localizer to map an input source number to the form for the request.
        WriteLine("public static partial bool TryGetCardinalRuleByLocale(string locale, [NotNullWhen(true)] out ContextPluralizer? contextPluralizer)");
        WriteLine("{");
        AddIndent();

        WriteLine("if (!Pluralizers.TryGetValue(locale, out var pluralizersForLocale))");
        WriteLine("{");
        AddIndent();
        WriteLine("contextPluralizer = null;");
        WriteLine("return false;");
        DecreaseIndent();
        WriteLine("}");
        WriteLine("contextPluralizer = pluralizersForLocale.Cardinal;");
        WriteLine("return contextPluralizer != null;");

        DecreaseIndent();
        WriteLine("}");
        WriteLine(string.Empty);

        // Repeat the above for ordinal rules.
        WriteLine("public static partial bool TryGetOrdinalRuleByLocale(string locale, [NotNullWhen(true)] out ContextPluralizer? contextPluralizer)");
        WriteLine("{");
        AddIndent();

        WriteLine("if (!Pluralizers.TryGetValue(locale, out var pluralizersForLocale))");
        WriteLine("{");
        AddIndent();
        WriteLine("contextPluralizer = null;");
        WriteLine("return false;");
        DecreaseIndent();
        WriteLine("}");
        WriteLine("contextPluralizer = pluralizersForLocale.Ordinal;");
        WriteLine("return contextPluralizer != null;");

        DecreaseIndent();
        WriteLine("}");

        // Generate the helper record and then clean up.
        WriteLine(string.Empty);
        WriteLine("private record LocalePluralizers(ContextPluralizer? Cardinal, ContextPluralizer? Ordinal);");

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