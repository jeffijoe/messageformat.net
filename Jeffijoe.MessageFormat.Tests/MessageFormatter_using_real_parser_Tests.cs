// MessageFormatter_using_real_parser_Tests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests
{
    public class MessageFormatter_using_real_parser_Tests
    {
        [Theory]
        [InlineData(@"Hi, I'm {name}, and it's still {name,plural,whatever

i do what i want

still inside braces

}, ok?", "Hi, I'm Jeff, and it's still Jeff, ok?")]
        public void FormatMessage_using_real_parser_and_library_mock(string source, string expected)
        {
            var mockLibary = new Mock<IFormatterLibrary>();
            var dummyFormatter = new Mock<IFormatter>();
            var subject = new MessageFormatter(new PatternParser(new LiteralParser()), mockLibary.Object, "en");

            var args = new Dictionary<string, object>();
            args.Add("name", "Jeff");
            dummyFormatter.Setup(x => x.Format("en", It.IsAny<FormatterRequest>(), args, subject)).Returns("Jeff");
            mockLibary.Setup(x => x.GetFormatter(It.IsAny<FormatterRequest>())).Returns(dummyFormatter.Object);

            Assert.Equal(expected, subject.FormatMessage(source, args));
        }
    }
}