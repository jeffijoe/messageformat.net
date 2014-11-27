// PatternParser_GetKey_Tests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

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
                yield return new object[]
                {
                    new Literal(3, 10, 1, 3, new StringBuilder("Hellåw,")),
                    1, 
                    8
                };
                yield return new object[]
                {
                    new Literal(0, 0, 3, 3, new StringBuilder(",")),
                    3, 
                    4
                };
                yield return new object[]
                {
                    new Literal(0, 0, 3, 3, new StringBuilder(" hello dawg")),
                    0, 
                    0
                };
                yield return new object[]
                {
                    new Literal(0, 0, 3, 3, new StringBuilder("hello dawg ")),
                    0, 
                    0
                };
                yield return new object[]
                {
                    new Literal(0, 0, 3, 3, new StringBuilder(" hello dawg")),
                    0, 
                    0
                };
                yield return new object[]
                {
                    new Literal(0, 0, 3, 3, new StringBuilder(" hello\r\ndawg")),
                    0, 
                    0
                };
            }
        }

        [Theory]
        [PropertyData("GetKey_throws_with_invalid_characters_Case")]
        public void ReadLiteralSection_throws_with_invalid_characters(Literal literal, int expectedLine, int expectedColumn)
        {
            var ex = Assert.Throws<MalformedLiteralException>(() => PatternParser.ReadLiteralSection(literal));
            Assert.Equal(expectedLine, ex.LineNumber);
            Assert.Equal(expectedColumn, ex.ColumnNumber);
            Console.WriteLine(ex.Message);
        }

        [Theory]
        [InlineData("SupDawg, yeah", "SupDawg")]
        [InlineData("hello", "hello")]
        [InlineData(" hello ", "hello")]
        [InlineData("0,", "0")]
        [InlineData("0, ", "0")]
        [InlineData("0 ,", "0")]
        [InlineData("0", "0")]
        public void ReadLiteralSection(string source, string expected)
        {
            Assert.Equal(expected, PatternParser.ReadLiteralSection(new Literal(10, 10, 1, 1, new StringBuilder(source))));
        }

        [Theory]
        [InlineData("SupDawg, yeah", "yeah", 8)]
        [InlineData("SupDawg,yeah", "yeah", 8)]
        [InlineData("SupDawg,yeah ", "yeah", 8)]
        [InlineData("SupDawg, ", null, 8)]
        [InlineData("SupDawg,", null, 8)]
        public void ReadLiteralSection_with_offset(string source, string expected, int offset)
        {
            Assert.Equal(expected, PatternParser.ReadLiteralSection(new Literal(10, 10, 1, 1, new StringBuilder(source)), offset, true));
        }
    }
}