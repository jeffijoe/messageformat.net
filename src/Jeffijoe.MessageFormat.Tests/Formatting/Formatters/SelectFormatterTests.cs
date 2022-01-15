// MessageFormat for .NET
// - SelectFormatterTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters
{
    /// <summary>
    /// The select formatter tests.
    /// </summary>
    public class SelectFormatterTests
    {
        #region Public Properties

        /// <summary>
        /// Gets the format_tests.
        /// </summary>
        public static IEnumerable<object[]> FormatTests
        {
            get
            {
                yield return new object[] { "male {he said} female {she said} other {they said}", "male", "he said" };
                yield return new object[]
                    { "male {he said} female {she said} other {they said}", "female", "she said" };
                yield return new object[] { "male {he said} female {she said} other {they said}", "dawg", "they said" };
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The format.
        /// </summary>
        /// <param name="formatterArgs">
        /// The formatter args.
        /// </param>
        /// <param name="gender">
        /// The gender.
        /// </param>
        /// <param name="expectedBlock">
        /// The expected block.
        /// </param>
        [Theory]
        [MemberData(nameof(FormatTests))]
        public void Format(string formatterArgs, string gender, string expectedBlock)
        {
            var subject = new SelectFormatter();
            var messageFormatter = new FakeMessageFormatter();
            var req = new FormatterRequest(
                new Literal(1, 1, 1, 1, ""),
                "gender",
                "select",
                formatterArgs);
            var args = new Dictionary<string, object?> { { "gender", gender } };
            var result = subject.Format("en", req, args, gender, messageFormatter);
            Assert.Equal(expectedBlock, result);
        }

        /// <summary>
        /// Verifies that format throws when no other option is given.
        /// </summary>
        [Fact]
        public void VerifyFormatThrowsWhenNoOtherOptionIsGiven()
        {
            var subject = new SelectFormatter();
            var messageFormatter = new FakeMessageFormatter();
            var req = new FormatterRequest(
                new Literal(1, 1, 1, 1, ""),
                "gender",
                "select",
                "male {he} female{she}");
            var args = new Dictionary<string, object?> { { "gender", "non-binary" } };

            Assert.Throws<MessageFormatterException>(() =>
            {
                subject.Format("en", req, args, "non-binary", messageFormatter);
            });
        }

        #endregion
    }
}