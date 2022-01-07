// MessageFormat for .NET
// - SelectFormatterTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;

using Moq;

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
                yield return new object[] { "male {he said} female {she said} other {they said}", "female", "she said" };
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
            var messageFormatterMock = new Mock<IMessageFormatter>();
            messageFormatterMock.Setup(x => x.FormatMessage(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
                                .Returns((string input, Dictionary<string, object> _) => input);
            var req = new FormatterRequest(
                new Literal(1, 1, 1, 1, ""), 
                "gender", 
                "select", 
                formatterArgs);
            var args = new Dictionary<string, object?> { { "gender", gender } };
            var result = subject.Format("en", req, args, gender, messageFormatterMock.Object);
            Assert.Equal(expectedBlock, result);
        }

        #endregion
    }
}