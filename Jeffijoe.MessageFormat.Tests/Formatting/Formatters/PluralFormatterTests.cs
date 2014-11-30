// PluralFormatterTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters
{
    public class PluralFormatterTests
    {
        [Theory]
        [InlineData(0, "nothing")]
        [InlineData(1, "just one")]
        [InlineData(1337, "wow")]
        public void Pluralize(double  n, string expected)
        {
            var subject = new PluralFormatter();
            var args = new Dictionary<string, object>
            {
                {"test", n}
            };
            var arguments = new ParsedArguments(
                new[]
                {
                    new KeyedBlock("zero", "nothing"), 
                    new KeyedBlock("one", "just one"), 
                    new KeyedBlock("other", "wow"), 
                }
                , new FormatterExtension[0]);
            var request = new FormatterRequest(
                new Literal(1,1,1,1, new StringBuilder()),
                "test", "plural", null
            );
            var actual = subject.Pluralize("en", request, arguments, args, Convert.ToDouble(args[request.Variable]));
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(@"Number \#1 has # results", "Number #1 has 1337 results")]
        public void ReplaceNumberLiterals(string input, string expected)
        {
            var subject = new PluralFormatter();
            var actual = subject.ReplaceNumberLiterals(new StringBuilder(input), 1337);
            Assert.Equal(expected, actual);
        }
    }
}