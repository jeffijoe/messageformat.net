// MessageFormat for .NET
// - MessageFormatter_caching_tests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Tests.TestHelpers;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    /// The message formatter_caching_tests.
    /// </summary>
    public class MessageFormatterCachingTests()
    {
        #region Public Methods and Operators

        /// <summary>
        /// The format message_caches_reused_pattern.
        /// </summary>
        [Fact]
        public void FormatMessage_caches_reused_pattern()
        {
            var parser = new TrackingPatternParser();
            var library = new FormatterLibrary();

            var subject = new MessageFormatter(patternParser: parser, library: library, useCache: true);

            var pattern = "Hi {gender, select, male {Sir} female {Ma'am}}!";
            var actual = subject.FormatMessage(pattern, new { gender = "male" });
            Assert.Equal("Hi Sir!", actual);

            // '2' because it did not format "Ma'am" yet.
            Assert.Equal(2, parser.ParseCount);

            actual = subject.FormatMessage(pattern, new { gender = "female" });
            Assert.Equal("Hi Ma'am!", actual);
            Assert.Equal(3, parser.ParseCount);

            // '3' because it has cached all options
            actual = subject.FormatMessage(pattern, new { gender = "female" });
            Assert.Equal("Hi Ma'am!", actual);
            Assert.Equal(3, parser.ParseCount);
        }
        
        #endregion
    }
}