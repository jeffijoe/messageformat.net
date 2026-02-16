using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator;

public class GeneratedPluralRulesTests
{
    [Theory]
    [InlineData(0, "днів")]
    [InlineData(1, "день")]
    [InlineData(101, "день")]
    [InlineData(102, "дні")]
    [InlineData(105, "днів")]
    public void Uk_PluralizerTests(double n, string expected)
    {
        var subject = new PluralFormatter();
        subject.Pluralizers.Clear();

        var args = new Dictionary<string, object> { { "test", n } };
        var arguments =
            new ParsedArguments(
                new[]
                {
                    new KeyedBlock("one", "день"),
                    new KeyedBlock("few", "дні"),
                    new KeyedBlock("many", "днів"),
                    new KeyedBlock("other", "дня")
                },
                new FormatterExtension[0]);
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", PluralFormatter.PluralFunction, null);
        var actual = subject.Pluralize(PluralRuleKey.Cardinal("uk"), arguments, new PluralContext(Convert.ToDecimal(Convert.ToDouble(args[request.Variable]))), 0);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, "дней")]
    [InlineData(1, "день")]
    [InlineData(101, "день")]
    [InlineData(102, "дня")]
    [InlineData(105, "дней")]
    public void Ru_PluralizerTests(double n, string expected)
    {
        var subject = new PluralFormatter();
        subject.Pluralizers.Clear();

        var args = new Dictionary<string, object> { { "test", n } };
        var arguments =
            new ParsedArguments(
                new[]
                {
                    new KeyedBlock("one", "день"),
                    new KeyedBlock("few", "дня"),
                    new KeyedBlock("many", "дней"),
                    new KeyedBlock("other", "дня")
                },
                new FormatterExtension[0]);
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", PluralFormatter.PluralFunction, null);
        var actual = subject.Pluralize(PluralRuleKey.Cardinal("ru"), arguments, new PluralContext(Convert.ToDecimal(args[request.Variable])), 0);
        Assert.Equal(expected, actual);
    }
        
    [Theory]
    [InlineData(0, "days")]
    [InlineData(1, "day")]
    [InlineData(101, "days")]
    [InlineData(102, "days")]
    [InlineData(105, "days")]
    public void En_Cardinal_PluralizerTests(double n, string expected)
    {
        var subject = new PluralFormatter();
        subject.Pluralizers.Clear();

        var args = new Dictionary<string, object> { { "test", n } };
        var arguments =
            new ParsedArguments(
                new[]
                {
                    // 'zero' is a red herring to confirm CLDR rules are used instead of built-in;
                    // CLDR does not specify an English 'zero' form, so 0 should fallthrough to 'other'.
                    new KeyedBlock("zero", "FAIL"),
                    new KeyedBlock("one", "day"),
                    new KeyedBlock("other", "days")
                },
                new FormatterExtension[0]);
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", PluralFormatter.PluralFunction, null);
        var actual = subject.Pluralize(PluralRuleKey.Cardinal("en"), arguments, new PluralContext(Convert.ToDecimal(args[request.Variable])), 0);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, "0th")]
    [InlineData(1, "1st")]
    [InlineData(2, "2nd")]
    [InlineData(3, "3rd")]
    [InlineData(4, "4th")]
    [InlineData(9, "9th")]
    [InlineData(11, "11th")]
    [InlineData(21, "21st")]
    public void En_Ordinal_PluralizerTests(double n, string expected)
    {
        var subject = new PluralFormatter();
        subject.Pluralizers.Clear();

        var args = new Dictionary<string, object> { { "test", n } };
        var arguments =
            new ParsedArguments(
                new[]
                {
                    new KeyedBlock("one", "#st"),
                    new KeyedBlock("two", "#nd"),
                    new KeyedBlock("few", "#rd"),
                    new KeyedBlock("other", "#th"),
                },
                new FormatterExtension[0]);
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", PluralFormatter.OrdinalFunction, null);
        var pluralized = subject.Pluralize(PluralRuleKey.Ordinal("en"), arguments, new PluralContext(Convert.ToDecimal(args[request.Variable])), 0);
        var actual = subject.ReplaceNumberLiterals(pluralized, n);
        Assert.Equal(expected, actual);
    }
}