// MessageFormat for .NET
// - PluralFormatterTests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters;

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
                Array.Empty<FormatterExtension>());
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "plural", null);
        var actual = subject.Pluralize("en", arguments, new PluralContext(Convert.ToDecimal(Convert.ToDouble(args[request.Variable]))), 0);
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// The pluralize_defaults_to_en_locale_when_specified_locale_is_not_found
    /// </summary>
    [Fact]
    public void Pluralize_defaults_to_en_locale_when_specified_locale_is_not_found()
    {
        var subject = new PluralFormatter();
        var args = new Dictionary<string, object> { { "test", 1 } };
        var arguments =
            new ParsedArguments(
                new[]
                {
                    new KeyedBlock("zero", "nothing"),
                    new KeyedBlock("one", "just one"),
                    new KeyedBlock("other", "wow")
                },
                Array.Empty<FormatterExtension>());
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "plural", null);
        var actual = subject.Pluralize("unknown", arguments, new PluralContext(Convert.ToDecimal(Convert.ToDouble(args[request.Variable]))), 0);
        Assert.Equal("just one", actual);
    }
        
    /// <summary>
    /// The pluralize_defaults_to_en_locale_when_specified_locale_is_not_found
    /// </summary>
    [Fact]
    public void Pluralize_throws_when_missing_other_block()
    {
        var subject = new PluralFormatter();
        var args = new Dictionary<string, object> { { "test", 5 } };
        var arguments =
            new ParsedArguments(
                new[]
                {
                    new KeyedBlock("zero", "nothing"),
                    new KeyedBlock("one", "just one")
                },
                Array.Empty<FormatterExtension>());
        var request = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "plural", null);
        Assert.Throws<MessageFormatterException>(() => subject.Pluralize("unknown", arguments, new PluralContext(Convert.ToDecimal(Convert.ToDouble(args[request.Variable]))), 0));
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
    [InlineData(@"Number '''#'''# has # results", "Number '''#'''1337 has 1337 results")]
    [InlineData(@"# results", "1337 results")]
    public void ReplaceNumberLiterals(string input, string expected)
    {
        var subject = new PluralFormatter();
        var actual = subject.ReplaceNumberLiterals(input, 1337);
        Assert.Equal(expected, actual);
    }

    #endregion
}