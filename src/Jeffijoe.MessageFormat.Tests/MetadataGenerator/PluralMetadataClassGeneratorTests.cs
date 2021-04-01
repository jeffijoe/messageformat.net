using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator
{
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
    public static partial class PluralRulesMetadata
    {
        public static string Locale_EN(double value) => Rule0(value);
        
        public static string Locale_UK(double value) => Rule0(value);
        
        private static string Rule0(double value)
        {
            var n = Math.Abs(value);
            
            if ((n == 3))
                return ""one"";
            
            return ""other"";
        }
        
        private static readonly Dictionary<string, Pluralizer> Pluralizers = new Dictionary<string, Pluralizer>()
        {
            {""en"", (Pluralizer) Rule0},
            {""uk"", (Pluralizer) Rule0},
            
        };
        
        public static partial bool TryGetRuleByLocale(string locale, out Pluralizer pluralizer)
        {
            return Pluralizers.TryGetValue(locale, out pluralizer);
        }
    }
}
".TrimStart();

            Assert.Equal(expected, actual);
        }
    }
}
