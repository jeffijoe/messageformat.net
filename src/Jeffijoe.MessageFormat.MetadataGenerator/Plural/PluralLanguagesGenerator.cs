using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

using Microsoft.CodeAnalysis;

using System.IO;
using System.Xml;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural;

[Generator]
public class PluralLanguagesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static spc =>
        {
            // Not currently excluding any locales.
            var rules = GetRules(excludedLocales: []);
            var generator = new PluralRulesMetadataGenerator(rules);
            var sourceCode = generator.GenerateClass();

            spc.AddSource("PluralRulesMetadata.Generated.cs", sourceCode);
        });
    }

    private static PluralRuleSet GetRules(string[] excludedLocales)
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

    private static Stream GetRulesContentStream(string cldrFileName)
    {
        return typeof(PluralLanguagesGenerator).Assembly
            .GetManifestResourceStream($"Jeffijoe.MessageFormat.MetadataGenerator.data.{cldrFileName}")!;
    }
}
