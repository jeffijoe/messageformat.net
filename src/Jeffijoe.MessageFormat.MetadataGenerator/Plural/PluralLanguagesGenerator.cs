using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural
{
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

        private IReadOnlyList<PluralRule> GetRules(string[] excludedLocales)
        {
            using var rulesStream = GetRulesContentStream();
            var xml = new XmlDocument();
            xml.Load(rulesStream);

            var parser = new PluralParser(xml, excludedLocales);
            return parser.Parse().ToArray();
        }


        private Stream GetRulesContentStream()
        {
            return typeof(PluralLanguagesGenerator).Assembly.GetManifestResourceStream("Jeffijoe.MessageFormat.MetadataGenerator.data.plurals.xml")!;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            
        }
    }
}
