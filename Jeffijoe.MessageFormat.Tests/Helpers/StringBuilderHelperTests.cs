// MessageFormat for .NET
// - StringBuilderHelperTests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Text;

using Jeffijoe.MessageFormat.Helpers;

using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Helpers
{
    public class StringBuilderHelperTests
    {
        [Theory]
        [InlineData("  dawg  ", "dawg")]
        [InlineData("  dawg  dawg  ", "dawg  dawg")]
        [InlineData("  dawg  dawg", "dawg  dawg")]
        [InlineData("dawg  dawg  ", "dawg  dawg")]
        [InlineData(" dawg  dawg  ", "dawg  dawg")]
        [InlineData(" dawg  dawg", "dawg  dawg")]
        [InlineData("dawg  dawg", "dawg  dawg")]
        public void TrimWhitespace(string input, string expected)
        {
            string actual = new StringBuilder(input).TrimWhitespace().ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("hello ", ' ', true)]
        [InlineData("hello ", 'l', true)]
        [InlineData("hello ", 'p', false)]
        public void Contains(string src, char c, bool expected)
        {
            Assert.Equal(expected, new StringBuilder(src).Contains(c));
        }

        [Theory]
        [InlineData(" hello", true)]
        [InlineData(" hello ", true)]
        [InlineData("hello ", true)]
        [InlineData("Hi", false)]
        public void ContainsWhitespace(string src, bool expected)
        {
            Assert.Equal(expected, new StringBuilder(src).ContainsWhitespace());
        }
    }
}