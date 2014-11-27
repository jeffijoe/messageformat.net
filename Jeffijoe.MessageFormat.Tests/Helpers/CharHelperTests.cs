// CharHelperTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using Jeffijoe.MessageFormat.Helpers;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Helpers
{
    public class CharHelperTests
    {
        [Fact]
        public void IsAlphaNumeric()
        {
            Assert.True('a'.IsAlphaNumeric());
            Assert.True('A'.IsAlphaNumeric());
            Assert.True('0'.IsAlphaNumeric());
            Assert.True('1'.IsAlphaNumeric());
            Assert.False('ä'.IsAlphaNumeric());
            Assert.False('ø'.IsAlphaNumeric());
            Assert.False('æ'.IsAlphaNumeric());
            Assert.False('å'.IsAlphaNumeric());
        }

        [Fact]
        public void IsWhitespace()
        {
            Assert.True(char.IsWhiteSpace(' '));
            Assert.True(char.IsWhiteSpace('\r'));
            Assert.True(char.IsWhiteSpace('\n'));
            Assert.True(char.IsWhiteSpace('\t'));
            Assert.False(char.IsWhiteSpace('1'));
        }
    }
}