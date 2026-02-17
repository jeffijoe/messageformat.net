using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator;

public class PluralMetadataClassGeneratorTests
{
    [Fact]
    public void CanGenerateClassFromRules()
    {
        var rules = new[]
        {
            new PluralRule(new[] {"root", "en", "uk"},
                new[]
                {
                    new Condition("one", string.Empty, new []
                    {
                        new OrCondition(new[]
                        {
                            new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] {new NumberOperand(3) })
                        })
                    })
                }),
            new PluralRule(new[] {"root", "en", "pt_PT"},
                new[]
                {
                    new Condition("many", string.Empty, new []
                    {
                        new OrCondition(new[]
                        {
                            new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] {new NumberOperand(120) })
                        })
                    })
                }),
        };

        var ruleSet = new PluralRuleSet();
        ruleSet.Add("cardinal", rules[0]);
        ruleSet.Add("ordinal", rules[1]);

        var generator = new PluralRulesMetadataGenerator(ruleSet);

        var actual = generator.GenerateClass();

        var expected = @"
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    internal static partial class PluralRulesMetadata
    {
        public static readonly string RootLocale = ""root"";
        private static string Rule0(PluralContext context)
        {
            if ((context.N == 3))
                return ""one"";
            
            return ""other"";
        }
        
        private static string Rule1(PluralContext context)
        {
            if ((context.N == 120))
                return ""many"";
            
            return ""other"";
        }
        
        private static readonly Dictionary<string, LocalePluralizers> Pluralizers = new(StringComparer.OrdinalIgnoreCase)
        {
            {""root"", new LocalePluralizers(Cardinal: Rule0, Ordinal: Rule1)},
            {""en"", new LocalePluralizers(Cardinal: Rule0, Ordinal: Rule1)},
            {""uk"", new LocalePluralizers(Cardinal: Rule0, Ordinal: null)},
            {""pt-PT"", new LocalePluralizers(Cardinal: null, Ordinal: Rule1)},
            {""pt_PT"", new LocalePluralizers(Cardinal: null, Ordinal: Rule1)},
        };
        
        public static partial bool TryGetCardinalRuleByLocale(string locale, [NotNullWhen(true)] out ContextPluralizer? contextPluralizer)
        {
            if (!Pluralizers.TryGetValue(locale, out var pluralizersForLocale))
            {
                contextPluralizer = null;
                return false;
            }
            contextPluralizer = pluralizersForLocale.Cardinal;
            return contextPluralizer != null;
        }
        
        public static partial bool TryGetOrdinalRuleByLocale(string locale, [NotNullWhen(true)] out ContextPluralizer? contextPluralizer)
        {
            if (!Pluralizers.TryGetValue(locale, out var pluralizersForLocale))
            {
                contextPluralizer = null;
                return false;
            }
            contextPluralizer = pluralizersForLocale.Ordinal;
            return contextPluralizer != null;
        }
        
        private record LocalePluralizers(ContextPluralizer? Cardinal, ContextPluralizer? Ordinal);
    }
}
".TrimStart();

        Assert.Equal(expected, actual);
    }
}