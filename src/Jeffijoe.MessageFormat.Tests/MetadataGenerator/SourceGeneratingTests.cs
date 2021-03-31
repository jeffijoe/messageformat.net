using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

using System;
using System.Text;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator
{
    public class SourceGeneratingTests
    {
        [Fact]
        public void CanGenerateEmptyRule()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, Array.Empty<Condition>()));

            var actual = GenerateText(generator);
            var expected = $"return \"other\";{Environment.NewLine}";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForFractionNumberEquals()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var v = (int)value == value ? 0 : 1;

if ((v == 0))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForIntegerDigitsEquals()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.IntegerDigits), Relation.Equals, new[] { new NumberOperand(1) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var i = (int)value;

if ((i == 1))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForModuloEquals()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new ModuloOperand(OperandSymbol.IntegerDigits, 5), Relation.Equals, new[] { new NumberOperand(1) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var i = (int)value;

if ((i % 5 == 1))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForNumberEquals()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new [] 
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(5) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);

if ((n == 5))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForNumberNotEquals()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.NotEquals, new[] { new NumberOperand(5) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);

if ((n != 5))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForNumberRange()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new IRightOperand[] { new RangeOperand(5, 6), new NumberOperand(10) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);

if ((n >= 5 && n <= 6 || n == 10))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForNegativeNumberRange()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.NotEquals, new IRightOperand[] { new RangeOperand(5, 6), new NumberOperand(10) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);

if (((n < 5 || n > 6) && n != 10))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForAndRules()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(4) }),
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);
var v = (int)value == value ? 0 : 1;

if ((n == 4) && (v == 0))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForMixedRangeAndNumberRangeRules()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new RangeOperand(4, 5) }),
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);
var v = (int)value == value ? 0 : 1;

if ((n >= 4 && n <= 5) && (v == 0))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForMultipleOrRules()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(4) })
                    }),
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);
var v = (int)value == value ? 0 : 1;

if ((n == 4) || (v == 0))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForMixedAndOrRules()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new NumberOperand(4) }),
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.NotEquals, new[] { new NumberOperand(0) })
                    }),
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);
var v = (int)value == value ? 0 : 1;

if ((n == 4) && (v != 0) || (v == 0))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanGenerateRuleForMixedAndOrRangeRules()
        {
            var generator = new RuleGenerator(new PluralRule(new[] { "en" }, new[]
            {
                new Condition("one", string.Empty, new[]
                {
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.AbsoluteValue), Relation.Equals, new[] { new RangeOperand(4, 5) }),
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.NotEquals, new[] { new NumberOperand(0) })
                    }),
                    new OrCondition(new []
                    {
                        new Operation(new VariableOperand(OperandSymbol.VisibleFractionDigitNumber), Relation.Equals, new[] { new NumberOperand(0) })
                    })
                })
            }));

            var actual = GenerateText(generator);
            var expected = @$"
var n = Math.Abs(value);
var v = (int)value == value ? 0 : 1;

if ((n >= 4 && n <= 5) && (v != 0) || (v == 0))
    return ""one"";

return ""other"";
".TrimStart();
            Assert.Equal(expected, actual);
        }

        private string GenerateText(RuleGenerator generator)
        {
            var sb = new StringBuilder();

            generator.WriteTo(sb, 0);

            return sb.ToString();
        }
    }
}
