using Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

using System;
using System.Text;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator;

public class RuleSourceGeneratorTests
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
if ((context.V == 0))
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
if ((context.I == 1))
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
if ((context.I % 5 == 1))
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
if ((context.N == 5))
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
if ((context.N != 5))
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
if ((context.N >= 5 && context.N <= 6 || context.N == 10))
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
if (((context.N < 5 || context.N > 6) && context.N != 10))
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
if ((context.N == 4) && (context.V == 0))
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
if ((context.N >= 4 && context.N <= 5) && (context.V == 0))
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
if ((context.N == 4) || (context.V == 0))
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
if ((context.N == 4) && (context.V != 0) || (context.V == 0))
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
if ((context.N >= 4 && context.N <= 5) && (context.V != 0) || (context.V == 0))
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