// MessageFormat for .NET
// - MessageFormatter_using_real_parser_Tests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests;

/// <summary>
/// The message formatter_using_real_parser_ tests.
/// </summary>
public class MessageFormatterUsingRealParserTests
{
    #region Fields

    /// <summary>
    /// The output helper.
    /// </summary>
    private ITestOutputHelper outputHelper;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageFormatterUsingRealParserTests"/> class.
    /// </summary>
    /// <param name="outputHelper">
    /// The output helper.
    /// </param>
    public MessageFormatterUsingRealParserTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The format message_using_real_parser_and_library_mock.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="expected">
    /// The expected.
    /// </param>
    [Theory]
    [InlineData(@"Hi, I'm {name}, and it's still {name, fake, whatever

i do what i want

still inside braces

bad boiis bad boiis

whatchu gonna do?

whatchu gonna do when dey come for youu?

}, ok?", "Hi, I'm Jeff, and it's still Jeff, ok?")]
    public void FormatMessage_using_real_parser_and_library_mock(string source, string expected)
    {
        var library = new FormatterLibrary();
        var dummyFormatter = new FakeFormatter(canFormat:true, formatResult: "Jeff");
        library.Add(dummyFormatter);
        var subject = new MessageFormatter(
            new PatternParser(new LiteralParser()), 
            library,
            false);

        var args = new Dictionary<string, object?>();
        args.Add("name", "Jeff");

        // Warm up
        Benchmark.Start("Warm-up", this.outputHelper);
        subject.FormatMessage(source, args);
        Benchmark.End(this.outputHelper);

        Benchmark.Start("Aaaand a few after warm-up", this.outputHelper);
        for (int i = 0; i < 1000; i++)
        {
            subject.FormatMessage(source, args);
        }

        Benchmark.End(this.outputHelper);

        Assert.Equal(expected, subject.FormatMessage(source, args));
    }

    #endregion
}