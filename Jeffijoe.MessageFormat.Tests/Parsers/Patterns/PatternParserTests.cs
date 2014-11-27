// PatternParserTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Parsers;
using Jeffijoe.MessageFormat.Parsers.Literals;
using Jeffijoe.MessageFormat.Parsers.Patterns;
using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Parsers.Patterns
{
    public class PatternParserTests
    {
        [Theory]
        [PropertyData("GetKey_throws_with_invalid_characters_Case")]
        public void GetKey_throws_with_invalid_characters(Literal literal, int expectedLine, int expectedColumn)
        {
            var ex = Assert.Throws<MalformedLiteralException>(() => PatternParser.GetKey(literal));
            Assert.Equal(expectedLine, ex.LineNumber);
            Assert.Equal(expectedColumn, ex.ColumnNumber);
            Console.WriteLine(ex.Message);
        }

        [Theory]
        [InlineData("SupDawg, yeah", "SupDawg")]
        [InlineData("hello", "hello")]
        [InlineData("0,", "0")]
        [InlineData("0", "0")]
        public void GetKey(string source, string expected)
        {
            Assert.Equal(expected, PatternParser.GetKey(new Literal(10, 10, 1, 1, new StringBuilder(source))).ToString());
        }

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
                    new Literal(0, 0, 3, 3, new StringBuilder("Lol\r\nDawg")),
                    3, 
                    7
                };
            }
        }
    }
}