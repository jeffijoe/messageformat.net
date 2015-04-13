// MessageFormat for .NET
// - PatternParser_GetKey_Tests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using Jeffijoe.MessageFormat.Parsing;

using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    public class PatternParser_GetKey_Tests
    {
        public static IEnumerable<object[]> GetKey_throws_with_invalid_characters_Case
        {
            get
            {
                yield return new object[] { new Literal(3, 10, 1, 3, new StringBuilder("Hellåw,")), 1, 8 };
                yield return new object[] { new Literal(0, 0, 3, 3, new StringBuilder(",")), 3, 4 };
                yield return new object[] { new Literal(0, 0, 3, 3, new StringBuilder(" hello dawg")), 0, 0 };
                yield return new object[] { new Literal(0, 0, 3, 3, new StringBuilder("hello dawg ")), 0, 0 };
                yield return new object[] { new Literal(0, 0, 3, 3, new StringBuilder(" hello dawg")), 0, 0 };
                yield return new object[] { new Literal(0, 0, 3, 3, new StringBuilder(" hello\r\ndawg")), 0, 0 };
            }
        }

        [Theory]
        [PropertyData("GetKey_throws_with_invalid_characters_Case")]
        public void ReadLiteralSection_throws_with_invalid_characters(
            Literal literal, 
            int expectedLine, 
            int expectedColumn)
        {
            int lastIndex;
            var ex =
                Assert.Throws<MalformedLiteralException>(
                    () => PatternParser.ReadLiteralSection(literal, 0, false, out lastIndex));
            Assert.Equal(expectedLine, ex.LineNumber);
            Assert.Equal(expectedColumn, ex.ColumnNumber);
            Console.WriteLine(ex.Message);
        }

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
            var literal = new Literal(10, 10, 1, 1, new StringBuilder(source));
            int lastIndex;
            Assert.Equal(expected, PatternParser.ReadLiteralSection(literal, 0, false, out lastIndex));
            Assert.Equal(expectedLastIndex, lastIndex);
        }

        [Theory]
        [InlineData("SupDawg, yeah", "yeah", 8)]
        [InlineData("SupDawg,yeah", "yeah", 8)]
        [InlineData("SupDawg,yeah ", "yeah", 8)]
        [InlineData("SupDawg, ", null, 8)]
        [InlineData("SupDawg,", null, 8)]
        public void ReadLiteralSection_with_offset(string source, string expected, int offset)
        {
            var literal = new Literal(10, 10, 1, 1, new StringBuilder(source));
            int lastIndex;
            Assert.Equal(expected, PatternParser.ReadLiteralSection(literal, offset, true, out lastIndex));
        }
    }
}