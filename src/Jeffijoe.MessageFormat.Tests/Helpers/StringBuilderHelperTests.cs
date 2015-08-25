// MessageFormat for .NET
// - StringBuilderHelperTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Text;

using Jeffijoe.MessageFormat.Helpers;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Helpers
{
    /// <summary>
    /// The string builder helper tests.
    /// </summary>
    public class StringBuilderHelperTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="src">
        /// The src.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [InlineData("hello ", ' ', true)]
        [InlineData("hello ", 'l', true)]
        [InlineData("hello ", 'p', false)]
        public void Contains(string src, char c, bool expected)
        {
            Assert.Equal(expected, new StringBuilder(src).Contains(c));
        }

        /// <summary>
        /// The contains whitespace.
        /// </summary>
        /// <param name="src">
        /// The src.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [InlineData(" hello", true)]
        [InlineData(" hello ", true)]
        [InlineData("hello ", true)]
        [InlineData("Hi", false)]
        public void ContainsWhitespace(string src, bool expected)
        {
            Assert.Equal(expected, new StringBuilder(src).ContainsWhitespace());
        }

        /// <summary>
        /// The trim whitespace.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
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

        #endregion
    }
}