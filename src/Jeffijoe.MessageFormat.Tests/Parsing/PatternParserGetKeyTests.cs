// MessageFormat for .NET
// - PatternParser_GetKey_Tests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests.Parsing;

/// <summary>
/// The pattern parser_ get key_ tests.
/// </summary>
public class PatternParserGetKeyTests
{
    #region Fields

    /// <summary>
    /// The output helper.
    /// </summary>
    private ITestOutputHelper outputHelper;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternParserGetKeyTests"/> class.
    /// </summary>
    /// <param name="outputHelper">
    /// The output helper.
    /// </param>
    public PatternParserGetKeyTests(ITestOutputHelper outputHelper)
    {
        this.outputHelper = outputHelper;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the get key_throws_with_invalid_characters_ case.
    /// </summary>
    public static IEnumerable<object[]> GetKeyThrowsWithInvalidCharactersCase
    {
        get
        {
            yield return new object[] { new Literal(3, 10, 1, 3, "Hellåw,"), 1, 8 };
            yield return new object[] { new Literal(0, 0, 3, 3, ","), 3, 4 };
            yield return new object[] { new Literal(0, 0, 3, 3, " hello dawg"), 0, 0 };
            yield return new object[] { new Literal(0, 0, 3, 3, "hello dawg "), 0, 0 };
            yield return new object[] { new Literal(0, 0, 3, 3, " hello dawg"), 0, 0 };
            yield return new object[] { new Literal(0, 0, 3, 3, " hello\r\ndawg"), 0, 0 };
        }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The read literal section.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="expected">
    /// The expected.
    /// </param>
    /// <param name="expectedLastIndex">
    /// The expected last index.
    /// </param>
    [Theory]
    [InlineData("SupDawg, yeah", "SupDawg", 7)]
    [InlineData("hello", "hello", 4)]
    [InlineData(" hello ", "hello", 6)]
    [InlineData("\r\nhello ", "hello", 7)]
    [InlineData("0,", "0", 1)]
    [InlineData("0, ", "0", 1)]
    [InlineData("0 ,", "0", 2)]
    [InlineData("0", "0", 0)]
    public void ReadLiteralSection(string source, string expected, int expectedLastIndex)
    {
        var literal = new Literal(10, 10, 1, 1, source);
        int lastIndex;
        Assert.Equal(expected, PatternParser.ReadLiteralSection(literal, 0, false, out lastIndex));
        Assert.Equal(expectedLastIndex, lastIndex);
    }

    /// <summary>
    /// The read literal section_throws_with_invalid_characters.
    /// </summary>
    /// <param name="literal">
    /// The literal.
    /// </param>
    /// <param name="expectedLine">
    /// The expected line.
    /// </param>
    /// <param name="expectedColumn">
    /// The expected column.
    /// </param>
    [Theory]
    [MemberData(nameof(GetKeyThrowsWithInvalidCharactersCase))]
    public void ReadLiteralSection_throws_with_invalid_characters(
        Literal literal, 
        int expectedLine, 
        int expectedColumn)
    {
        var ex =
            Assert.Throws<MalformedLiteralException>(
                () => PatternParser.ReadLiteralSection(literal, 0, false, out _));
        Assert.Equal(expectedLine, ex.LineNumber);
        Assert.Equal(expectedColumn, ex.ColumnNumber);
        this.outputHelper.WriteLine(ex.Message);
    }

    /// <summary>
    /// The read literal section_with_offset.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="expected">
    /// The expected.
    /// </param>
    /// <param name="offset">
    /// The offset.
    /// </param>
    [Theory]
    [InlineData("SupDawg, yeah", "yeah", 8)]
    [InlineData("SupDawg,yeah", "yeah", 8)]
    [InlineData("SupDawg,yeah ", "yeah", 8)]
    [InlineData("SupDawg, ", null, 8)]
    [InlineData("SupDawg,", null, 8)]
    public void ReadLiteralSection_with_offset(string source, string? expected, int offset)
    {
        var literal = new Literal(10, 10, 1, 1, source);
        Assert.Equal(expected, PatternParser.ReadLiteralSection(literal, offset, true, out _));
    }

    #endregion
}