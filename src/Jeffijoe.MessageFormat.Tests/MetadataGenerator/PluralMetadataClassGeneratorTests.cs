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
                }),
            new PluralRule(new[] {"en"},
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
using System;
using System.Collections.Generic;
namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    internal static partial class PluralRulesMetadata
    {
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
        
        private static readonly Dictionary<PluralRuleKey, ContextPluralizer> Pluralizers = new Dictionary<PluralRuleKey, ContextPluralizer>()
        {
            {new PluralRuleKey(Locale: ""en"", PluralType: ""cardinal""), Rule0},
            {new PluralRuleKey(Locale: ""uk"", PluralType: ""cardinal""), Rule0},
            {new PluralRuleKey(Locale: ""en"", PluralType: ""ordinal""), Rule1},
        };
        
        public static partial bool TryGetRuleByLocale(PluralRuleKey key, out ContextPluralizer contextPluralizer)
        {
            return Pluralizers.TryGetValue(key, out contextPluralizer);
        }
    }
}
".TrimStart();

        Assert.Equal(expected, actual);
    }
}