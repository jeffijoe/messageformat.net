// MessageFormat for .NET
// - MessageFormatter_using_real_parser_Tests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Moq;

using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests
{
    public class MessageFormatter_using_real_parser_Tests
    {
        [Theory]
        [InlineData(@"Hi, I'm {name}, and it's still {name, plural, whatever

i do what i want

still inside braces

bad boiis bad boiis

whatchu gonna do?

whatchu gonna do when dey come for youu?

}, ok?", "Hi, I'm Jeff, and it's still Jeff, ok?")]
        public void FormatMessage_using_real_parser_and_library_mock(string source, string expected)
        {
            var mockLibary = new Mock<IFormatterLibrary>();
            var dummyFormatter = new Mock<IFormatter>();
            var subject = new MessageFormatter(
                new PatternParser(new LiteralParser()), 
                mockLibary.Object, 
                false, 
                locale: "en");

            var args = new Dictionary<string, object>();
            args.Add("name", "Jeff");
            dummyFormatter.Setup(x => x.Format("en", It.IsAny<FormatterRequest>(), args, subject)).Returns("Jeff");
            mockLibary.Setup(x => x.GetFormatter(It.IsAny<FormatterRequest>())).Returns(dummyFormatter.Object);

            // Warm up
            Benchmark.Start("Warm-up");
            subject.FormatMessage(source, args);
            Benchmark.End();

            Benchmark.Start("Aaaand a few after warm-up");
            for (int i = 0; i < 1000; i++)
            {
                subject.FormatMessage(source, args);
            }

            Benchmark.End();

            Assert.Equal(expected, subject.FormatMessage(source, args));
        }
    }
}