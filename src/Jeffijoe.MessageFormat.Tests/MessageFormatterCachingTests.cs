// MessageFormat for .NET
// - MessageFormatter_caching_tests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Parsing;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    /// The message formatter_caching_tests.
    /// </summary>
    public class MessageFormatterCachingTests
    {
        #region Fields

        /// <summary>
        /// The output helper.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatterCachingTests"/> class.
        /// </summary>
        /// <param name="outputHelper">
        /// The output helper.
        /// </param>
        public MessageFormatterCachingTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The format message_caches_reused_pattern.
        /// </summary>
        [Fact]
        public void FormatMessage_caches_reused_pattern()
        {
            var parserMock = new Mock<IPatternParser>();
            var realParser = new PatternParser(new LiteralParser());
            parserMock.Setup(x => x.Parse(It.IsAny<StringBuilder>()))
                      .Returns((StringBuilder sb) => realParser.Parse(sb));
            var library = new FormatterLibrary();

            var subject = new MessageFormatter(patternParser: parserMock.Object, library: library, useCache: true);

            var pattern = "Hi {gender, select, male {Sir} female {Ma'am}}!";
            var actual = subject.FormatMessage(pattern, new { gender = "male" });
            Assert.Equal("Hi Sir!", actual);

            // '2' because it did not format "Ma'am" yet.
            parserMock.Verify(x => x.Parse(It.IsAny<StringBuilder>()), Times.Exactly(2));

            actual = subject.FormatMessage(pattern, new { gender = "female" });
            Assert.Equal("Hi Ma'am!", actual);
            parserMock.Verify(x => x.Parse(It.IsAny<StringBuilder>()), Times.Exactly(3));

            // '3' because it has cached all options
            actual = subject.FormatMessage(pattern, new { gender = "female" });
            Assert.Equal("Hi Ma'am!", actual);
            parserMock.Verify(x => x.Parse(It.IsAny<StringBuilder>()), Times.Exactly(3));
        }

        /// <summary>
        /// The format message_with_cache_benchmark.
        /// </summary>
        [Fact]
        public void FormatMessage_with_cache_benchmark()
        {
            var subject = new MessageFormatter(true);
            this.Benchmark(subject);
        }

        /// <summary>
        /// The format message_without_cache_benchmark.
        /// </summary>
        [Fact]
        public void FormatMessage_without_cache_benchmark()
        {
            var subject = new MessageFormatter(false);
            this.Benchmark(subject);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The benchmark.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        private void Benchmark(MessageFormatter subject)
        {
            var pattern = "\r\n----\r\nOh {name}? And if we were " + "to surround {gender, select, " + "male {his} "
                          + "female {her}" + "} name with '{' and '}', it would look "
                          + "like '{'{name}'}'? Yeah, I know {gender, select, " + "male {him} " + "female {her}"
                          + "}. {gender, select, " + "male {He's}" + "female {She's}" + "} got {messageCount, plural, "
                          + "zero {no messages}" + "one {just one message}" + "=42 {a universal amount of messages}"
                          + "other {uuhm... let's see.. Oh yeah, # messages - and here's a pound: '#'}" + "}!";
            int iterations = 100000;
            var args = new Dictionary<string, object?>[iterations];
            var rnd = new Random();
            for (int i = 0; i < iterations; i++)
            {
                var val = rnd.Next(50);
                args[i] =
                    new
                    {
                        gender = val % 2 == 0 ? "male" : "female",
                        name = val % 2 == 0 ? "Jeff" : "Marcela",
                        messageCount = val
                    }.ToDictionary();
            }

            TestHelpers.Benchmark.Start("Formatting message " + iterations + " times, no warm-up.", this.outputHelper);
            var output = new StringBuilder();
            for (int i = 0; i < iterations; i++)
            {
                output.AppendLine(subject.FormatMessage(pattern, args[i]));
            }

            TestHelpers.Benchmark.End(this.outputHelper);
            this.outputHelper.WriteLine(output.ToString());
        }

        #endregion
    }
}