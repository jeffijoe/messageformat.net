// MessageFormat for .NET
// - MessageFormatter_caching_tests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Parsing;

using Moq;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    public class MessageFormatter_caching_tests
    {
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

        [Fact]
        public void FormatMessage_with_cache_benchmark()
        {
            var subject = new MessageFormatter(true);
            Benchmark(subject);
        }

        [Fact]
        public void FormatMessage_without_cache_benchmark()
        {
            var subject = new MessageFormatter(false);
            Benchmark(subject);
        }

        private static void Benchmark(MessageFormatter subject)
        {
            var pattern = "\r\n----\r\nOh {name}? And if we were " + "to surround {gender, select, " + "male {his} "
                          + "female {her}" + "} name with \\{ and \\}, it would look "
                          + "like \\{{name}\\}? Yeah, I know {gender, select, " + "male {him} " + "female {her}"
                          + "}. {gender, select, " + "male {He's}" + "female {She's}" + "} got {messageCount, plural, "
                          + "zero {no messages}" + "one {just one message}" + "=42 {a universal amount of messages}"
                          + "other {uuhm... let's see.. Oh yeah, # messages - and here's a pound: \\#}" + "}!";
            int iterations = 100000;
            var args = new Dictionary<string, object>[iterations];
            var rnd = new Random();
            for (int i = 0; i < iterations; i++)
            {
                var val = rnd.Next(50);
                args[i] =
                    new
                    {
                        gender = val % 2 == 0 ? "male" : "female", 
                        name = val % 2 == 0 ? "Jeff" : "Amanda", 
                        messageCount = val
                    }.ToDictionary();
            }

            TestHelpers.Benchmark.Start("Formatting message " + iterations + " times, no warm-up.");
            var output = new StringBuilder();
            for (int i = 0; i < iterations; i++)
            {
                output.AppendLine(subject.FormatMessage(pattern, args[i]));
            }

            TestHelpers.Benchmark.End();
            Console.WriteLine(output.ToString());
        }
    }
}
