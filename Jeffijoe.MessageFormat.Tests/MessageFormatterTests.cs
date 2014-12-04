// MessageFormatterTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Moq;
using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests
{
    public class MessageFormatterTests
    {
        [Fact]
        public void FormatMessage()
        {
            var patternParserMock = new Mock<IPatternParser>();
            var libraryMock = new Mock<IFormatterLibrary>();
            var collectionMock = new Mock<IFormatterRequestCollection>();
            var formatterMock1 = new Mock<IFormatter>();
            var formatterMock2 = new Mock<IFormatter>();
            var subject = new MessageFormatter(patternParserMock.Object, libraryMock.Object, "en");

            // I can use a bogus plural argument. Mocks <3
            var pattern = "{name} has {messages, plural, 123}.";
            var expected = "Jeff has 123 messages.";
            var args = new Dictionary<string, object>
            {
                {"name", "Jeff"},
                {"messages", 1}
            };
            var requests = new[]
            {
                new FormatterRequest(
                    new Literal(0, 5, 1, 7, new StringBuilder("name")), 
                    "name",
                    null,
                    null
                    ), 
                new FormatterRequest(
                    new Literal(11, 33, 1, 7, new StringBuilder("messages, plural, 123")), 
                    "messages",
                    "plural",
                    " 123"
                    )
            };
            formatterMock1.Setup(x => x.Format("en", requests[0], args, subject))
                .Returns("Jeff");
            formatterMock2.Setup(x => x.Format("en", requests[1], args, subject))
                .Returns("123 messages");

            collectionMock.Setup(x => x.GetEnumerator()).Returns(EnumeratorMock(requests));

            libraryMock.Setup(x => x.GetFormatter(requests[0]))
                .Returns(formatterMock1.Object);

            libraryMock.Setup(x => x.GetFormatter(requests[1]))
                .Returns(formatterMock2.Object);
            patternParserMock.Setup(x => x.Parse(It.IsAny<StringBuilder>()))
                .Returns(collectionMock.Object);
            
            // First request, and "name" is 4 chars.
            collectionMock.Setup(x => x.ShiftIndices(0, 4))
                .Callback((int index, int length) =>
                {
                    // The '- 2' is also done in the used implementation.
                    requests[1].SourceLiteral.ShiftIndices(length - 2, requests[0].SourceLiteral);
                });
            var actual = subject.FormatMessage(pattern, args);
            collectionMock.Verify(x => x.ShiftIndices(0, 4), Times.Once);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(@"Hello \{buddy\}, how are you \{doing\}?", "Hello {buddy}, how are you {doing}?")]
        [InlineData(@"Hello \\{buddy\\}, how are you \{doing\}?", @"Hello \{buddy\}, how are you {doing}?")]
        public void UnescapeLiterals(string source, string expected)
        {
            var patternParserMock = new Mock<IPatternParser>();
            var libraryMock = new Mock<IFormatterLibrary>();
            var subject = new MessageFormatter(patternParserMock.Object, libraryMock.Object, "en");

            var actual = subject.UnescapeLiterals(new StringBuilder(source)).ToString();
            Assert.Equal(expected, actual);
        }

        private IEnumerator<FormatterRequest> EnumeratorMock(IEnumerable<FormatterRequest> values)
        {
            foreach (var formatterRequest in values)
            {
                yield return formatterRequest;
            }
        }
    }
}