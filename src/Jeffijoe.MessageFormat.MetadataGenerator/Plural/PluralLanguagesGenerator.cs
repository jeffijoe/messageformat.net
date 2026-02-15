using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

using Microsoft.CodeAnalysis;

using System;
using System.IO;
using System.Xml;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural;

[Generator]
public class PluralLanguagesGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var excludeLocales = ReadExcludeLocales(context);
        var rules = GetRules(excludeLocales);
        var generator = new PluralRulesMetadataGenerator(rules);
        var sourceCode = generator.GenerateClass();

        context.AddSource("PluralRulesMetadata.Generated.cs", sourceCode);
    }

    private string[] ReadExcludeLocales(GeneratorExecutionContext context)
    {
        if(context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.PluralLanguagesMetadataExcludeLocales", out var value))
        {
            var locales = value.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return locales;
        }

        return Array.Empty<string>();
    }

    private PluralRuleSet GetRules(string[] excludedLocales)
    {
        PluralRuleSet ruleIndex = new();
        foreach (var ruleset in new[] { "plurals.xml", "ordinals.xml" })
        {
            using var rulesStream = GetRulesContentStream(ruleset);
            var xml = new XmlDocument();
            xml.Load(rulesStream);

            var parser = new PluralParser(xml, excludedLocales);
            parser.ParseInto(ruleIndex);
        }

        return ruleIndex;
    }


    private Stream GetRulesContentStream(string cldrFileName)
    {
        return typeof(PluralLanguagesGenerator).Assembly.GetManifestResourceStream($"Jeffijoe.MessageFormat.MetadataGenerator.data.{cldrFileName}")!;
    }

    public void Initialize(GeneratorInitializationContext context)
    {
            
    }
}