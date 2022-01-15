// MessageFormat for .NET
// - LiteralParserTests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Linq;
using System.Text;

using Jeffijoe.MessageFormat.Parsing;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    /// <summary>
    /// The literal parser tests.
    /// </summary>
    public class LiteralParserTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The parse literals_bracket_mismatch.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expectedOpenBraceCount">
        /// The expected open brace count.
        /// </param>
        /// <param name="expectedCloseBraceCount">
        /// The expected close brace count.
        /// </param>
        [Theory]
        [InlineData("{", 1, 0)]
        [InlineData("}", 0, 1)]
        [InlineData("A beginning {", 1, 0)]
        [InlineData("An ending }", 0, 1)]
        [InlineData("One { and multiple }}", 1, 2)]
        [InlineData("A few {{{{ and one }", 4, 1)]
        [InlineData("A few {{{{ and one '}'}", 4, 1)]
        [InlineData("A few '{'{{{{ and one '}'}", 4, 1)]
        public void ParseLiterals_bracket_mismatch(
            string source,
            int expectedOpenBraceCount,
            int expectedCloseBraceCount)
        {
            var sb = new StringBuilder(source);
            var subject = new LiteralParser();
            var ex = Assert.Throws<UnbalancedBracesException>(() => subject.ParseLiterals(sb));
            Assert.Equal(expectedOpenBraceCount, ex.OpenBraceCount);
            Assert.Equal(expectedCloseBraceCount, ex.CloseBraceCount);
        }

        /// <summary>
        /// The parse literals_count.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expectedMatchCount">
        /// The expected match count.
        /// </param>
        [Theory]
        [InlineData("Hello, {something smells {really} weird.}", 1)]
        [InlineData("Hello, {something smells {really} weird.}, {Hi}", 2)]
        [InlineData("Hello, {something smells {really} weird.}, '{Hi}'", 1)]
        public void ParseLiterals_count(string source, int expectedMatchCount)
        {
            var sb = new StringBuilder(source);
            var subject = new LiteralParser();
            var actual = subject.ParseLiterals(sb);
            Assert.Equal(expectedMatchCount, actual.Count());
        }

        /// <summary>
        /// The parse unclosed_escape_sequence.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expectedLineNumber">
        /// The expected line number.
        /// </param>
        /// <param name="expectedColumnNumber">
        /// The expected column number.
        /// </param>
        [Theory]
        [InlineData("'{", 1, 1)]
        [InlineData("'}", 1, 1)]
        [InlineData("a {b {c} d}, '{open escape sequence}", 1, 14)]
        [InlineData(@"Hello,
'{World}", 2, 1)]
        public void ParseLiterals_unclosed_escape_sequence(
            string source,
            int expectedLineNumber,
            int expectedColumnNumber)
        {
            var sb = new StringBuilder(source);
            var subject = new LiteralParser();
            var ex = Assert.Throws<MalformedLiteralException>(() => subject.ParseLiterals(sb));
            Assert.Equal(expectedLineNumber, ex.LineNumber);
            Assert.Equal(expectedColumnNumber, ex.ColumnNumber);
        }

        /// <summary>
        /// The parse literals_position_and_inner_text.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="expectedInnerText">
        /// The expected inner text.
        /// </param>
        [Theory]
        [InlineData("Hello, {something smells {really} weird.}", new[] { 7, 40 }, "something smells {really} weird.")]
        [InlineData("Pretty {sweet}, right?", new[] { 7, 13 }, "sweet")]
        [InlineData(@"{
sweet

}, right?", new[] { 0, 9 }, @"sweet")]
        [InlineData(@"{
'{sweet}'

}, right?", new[] { 0, 13 }, @"'{sweet}'")]
        public void ParseLiterals_position_and_inner_text(string source, int[] position, string expectedInnerText)
        {
            var sb = new StringBuilder(source);
            var subject = new LiteralParser();
            var actual = subject.ParseLiterals(sb);
            var first = actual.First();
            string innerText = first.InnerText;
            Assert.Equal(expectedInnerText, innerText);
            Assert.Equal(position[0], first.StartIndex);

            // Makes up for line-ending differences due to Git.
            var expectedEndIndex = position[1] + source.Count(c => c == '\r');
            var expectedSourceColumnNumber = first.StartIndex + 1;
            Assert.Equal(expectedEndIndex, first.EndIndex);

            Assert.Equal(expectedSourceColumnNumber, first.SourceColumnNumber);
        }

        /// <summary>
        /// The parse literals_source_line_and_column_number.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        [Theory]
        [InlineData(@"Hi, this is

{a tricky one}

yeeah!
", 3, 1)]
        [InlineData(@"Hi, this is

  
  {a tricky one}

yeeah!
", 4, 3)]
        public void ParseLiterals_source_line_and_column_number(string source, int lineNumber, int columnNumber)
        {
            var sb = new StringBuilder(source);
            var subject = new LiteralParser();
            var actual = subject.ParseLiterals(sb);
            var first = actual.First();
            Assert.Equal(lineNumber, first.SourceLineNumber);
            Assert.Equal(columnNumber, first.SourceColumnNumber);
        }

        #endregion
    }
}