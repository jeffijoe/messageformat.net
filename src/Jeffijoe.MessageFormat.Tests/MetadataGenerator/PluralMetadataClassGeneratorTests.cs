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
            new PluralRule(new[] {"en", "uk"},
                new[]
                {
                    new Condition("one", string.Empty, new []
                    {
                        new OrCondition(new[]
                        {
                            new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] {new NumberOperand(3) })
                        })
                    })
                })
        };
        var generator = new PluralRulesMetadataGenerator(rules);

        var actual = generator.GenerateClass();

        var expected = @"
using System;
using System.Collections.Generic;
namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    internal static partial class PluralRulesMetadata
    {
        public static string Locale_EN(PluralContext context) => Rule0(context);
        
        public static string Locale_UK(PluralContext context) => Rule0(context);
        
        private static string Rule0(PluralContext context)
        {
            if ((context.N == 3))
                return ""one"";
            
            return ""other"";
        }
        
        private static readonly Dictionary<string, ContextPluralizer> Pluralizers = new Dictionary<string, ContextPluralizer>()
        {
            {""en"", Rule0},
            {""uk"", Rule0},
            
        };
        
        public static partial bool TryGetRuleByLocale(string locale, out ContextPluralizer contextPluralizer)
        {
            return Pluralizers.TryGetValue(locale, out contextPluralizer);
        }
    }
}
".TrimStart();

        Assert.Equal(expected, actual);
    }
}