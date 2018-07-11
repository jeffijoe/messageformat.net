// MessageFormat for .NET
// - PluralFormatterTests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters
{
    /// <summary>
    /// The plural formatter tests.
    /// </summary>
    public class PluralFormatterTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The pluralize.
        /// </summary>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [InlineData(0, "nothing")]
        [InlineData(1, "just one")]
        [InlineData(1337, "wow")]
        public void Pluralize(double n, string expected)
        {
            var subject = new PluralFormatter();
            var args = new Dictionary<string, object> { { "test", n } };
            var arguments =
                new ParsedArguments(
                    new[]
                    {
                        new KeyedBlock("zero", "nothing"),
                        new KeyedBlock("one", "just one"),
                        new KeyedBlock("other", "wow")
                    },
                    new FormatterExtension[0]);
            var request = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), "test", "plural", null);
            var actual = subject.Pluralize("en", arguments, Convert.ToDouble(args[request.Variable]), 0);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// The replace number literals.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [InlineData(@"Number '#1' has # results", "Number '#1' has 1337 results")]
        [InlineData(@"Number '#'1 has # results", "Number '#'1 has 1337 results")]
        [InlineData(@"Number '#'# has # results", "Number '#'1337 has 1337 results")]
        [InlineData(@"# results", "1337 results")]
        public void ReplaceNumberLiterals(string input, string expected)
        {
            var subject = new PluralFormatter();
            var actual = subject.ReplaceNumberLiterals(new StringBuilder(input), 1337);
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}