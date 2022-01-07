using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator
{
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
            var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "plural", null);
            var actual = subject.Pluralize("uk", arguments, new PluralContext(Convert.ToDecimal(Convert.ToDouble(args[request.Variable]))), 0);
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
            var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "plural", null);
            var actual = subject.Pluralize("ru", arguments, new PluralContext(Convert.ToDecimal(args[request.Variable])), 0);
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData(0, "days")]
        [InlineData(1, "day")]
        [InlineData(101, "days")]
        [InlineData(102, "days")]
        [InlineData(105, "days")]
        public void En_PluralizerTests(double n, string expected)
        {
            var subject = new PluralFormatter();
            var args = new Dictionary<string, object> { { "test", n } };
            var arguments =
                new ParsedArguments(
                    new[]
                    {
                        new KeyedBlock("one", "day"),
                        new KeyedBlock("other", "days")
                    },
                    new FormatterExtension[0]);
            var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "plural", null);
            var actual = subject.Pluralize("en", arguments, new PluralContext(Convert.ToDecimal(args[request.Variable])), 0);
            Assert.Equal(expected, actual);
        }
    }
}