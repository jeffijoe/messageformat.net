// MessageFormat for .NET
// - MessageFormatterTests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;

using Moq;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    ///     The message formatter tests.
    /// </summary>
    public class MessageFormatterTests
    {
        #region Fields

        /// <summary>
        /// The collection mock.
        /// </summary>
        private readonly Mock<IFormatterRequestCollection> collectionMock;

        /// <summary>
        /// The formatter mock 1.
        /// </summary>
        private readonly Mock<IFormatter> formatterMock1;

        /// <summary>
        /// The formatter mock 2.
        /// </summary>
        private readonly Mock<IFormatter> formatterMock2;

        /// <summary>
        /// The library mock.
        /// </summary>
        private readonly Mock<IFormatterLibrary> libraryMock;

        /// <summary>
        /// The message formatter.
        /// </summary>
        private readonly MessageFormatter subject;

        /// <summary>
        /// The pattern parser mock.
        /// </summary>
        private readonly Mock<IPatternParser> patternParserMock;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatterTests"/> class.
        /// </summary>
        public MessageFormatterTests()
        {
            this.patternParserMock = new Mock<IPatternParser>();
            this.libraryMock = new Mock<IFormatterLibrary>();
            this.collectionMock = new Mock<IFormatterRequestCollection>();
            this.formatterMock1 = new Mock<IFormatter>();
            this.formatterMock2 = new Mock<IFormatter>();
            this.subject = new MessageFormatter(this.patternParserMock.Object, this.libraryMock.Object, false);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The format message.
        /// </summary>
        [Fact]
        public void FormatMessage()
        {
            const string Pattern = "{name} has {messages, plural, 123}.";
            const string Expected = "Jeff has 123 messages.";
            var args = new Dictionary<string, object?> { { "name", "Jeff" }, { "messages", 1 } };
            var requests = new[]
            {
                new FormatterRequest(
                    new Literal(0, 5, 1, 7, "name"),
                    "name",
                    null,
                    null),
                new FormatterRequest(
                    new Literal(11, 33, 1, 7, "messages, plural, 123"),
                    "messages",
                    "plural",
                    " 123")
            };

            this.formatterMock1.Setup(x => x.Format("en", requests[0], args, "Jeff", this.subject)).Returns("Jeff");
            this.formatterMock2.Setup(x => x.Format("en", requests[1], args, 1, this.subject)).Returns("123 messages");
            this.collectionMock.Setup(x => x.GetEnumerator()).Returns(requests.AsEnumerable().GetEnumerator());
            this.collectionMock.Setup(x => x.Count).Returns(requests.Length);
            this.collectionMock.Setup(x => x[It.IsAny<int>()]).Returns((int i) => requests[i]);
            this.libraryMock.Setup(x => x.GetFormatter(requests[0])).Returns(this.formatterMock1.Object);
            this.libraryMock.Setup(x => x.GetFormatter(requests[1])).Returns(this.formatterMock2.Object);
            this.patternParserMock.Setup(x => x.Parse(It.IsAny<StringBuilder>())).Returns(this.collectionMock.Object);

            // First request, and "name" is 4 chars.
            this.collectionMock.Setup(x => x.ShiftIndices(0, 4)).Callback(

                // The '- 2' is also done in the used implementation.
                (int index, int length) => requests[1].SourceLiteral.ShiftIndices(length - 2, requests[0].SourceLiteral));

            var actual = this.subject.FormatMessage(Pattern, args);
            this.collectionMock.Verify(x => x.ShiftIndices(0, 4), Times.Once);
            this.libraryMock.VerifyAll();
            this.formatterMock1.VerifyAll();
            this.formatterMock2.VerifyAll();
            this.patternParserMock.VerifyAll();
            Assert.Equal(Expected, actual);
        }

        /// <summary>
        /// The unescape literals.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [InlineData(@"Hello '{buddy}', how are you '{doing}'?", "Hello {buddy}, how are you {doing}?")]
        [InlineData(@"Hello ''{buddy}'', how are you '{doing}'?", @"Hello '{buddy}', how are you {doing}?")]
        public void UnescapeLiterals(string source, string expected)
        {
            var actual = this.subject.UnescapeLiterals(new StringBuilder(source)).ToString();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///     Verifies that format message throws when variables are missing and the formatter requires it to exist.
        /// </summary>
        [Fact]
        public void VerifyFormatMessageThrowsWhenVariablesAreMissingAndTheFormatterRequiresItToExist()
        {
            const string Pattern = "{name} has {messages, plural, 123}.";

            // Note the missing "name" variable.
            var args = new Dictionary<string, object?> { { "messages", 1 } };
            var requests = new[]
            {
                new FormatterRequest(
                    new Literal(0, 5, 1, 7, "name"),
                    "name",
                    null,
                    null),
                new FormatterRequest(
                    new Literal(11, 33, 1, 7, "messages, plural, 123"),
                    "messages",
                    "plural",
                    " 123")
            };

            this.collectionMock.Setup(x => x.GetEnumerator()).Returns(() => requests.AsEnumerable().GetEnumerator());
            this.collectionMock.Setup(x => x.Count).Returns(requests.Length);
            this.collectionMock.Setup(x => x[It.IsAny<int>()]).Returns((int i) => requests[i]);
            this.patternParserMock.Setup(x => x.Parse(It.IsAny<StringBuilder>())).Returns(this.collectionMock.Object);
            this.formatterMock1.SetupGet(x => x.VariableMustExist).Returns(true);
            this.libraryMock.Setup(x => x.GetFormatter(It.IsAny<FormatterRequest>())).Returns(formatterMock1.Object);
            
            // First request, and "name" is 4 chars.
            this.collectionMock.Setup(x => x.ShiftIndices(0, 4)).Callback(

                // The '- 2' is also done in the used implementation.
                (int index, int length) => requests[1].SourceLiteral.ShiftIndices(length - 2, requests[0].SourceLiteral));

            var ex = Assert.Throws<VariableNotFoundException>(() => this.subject.FormatMessage(Pattern, args));
            Assert.Equal("name", ex.MissingVariable);
        }
        
        /// <summary>
        ///     Verifies that format message allows non-existent variables when formatter allows it.
        /// </summary>
        [Fact]
        public void VerifyFormatMessageAllowsNonExistentVariablesWhenFormatterAllowsIt()
        {
            const string Pattern = "{name}";

            // Note the missing "name" variable.
            var args = new Dictionary<string, object?> ();
            var requests = new[]
            {
                new FormatterRequest(
                    new Literal(0, 5, 1, 7, "name"),
                    "name",
                    null,
                    null),
            };

            this.collectionMock.Setup(x => x.GetEnumerator()).Returns(() => requests.AsEnumerable().GetEnumerator());
            this.collectionMock.Setup(x => x.Count).Returns(requests.Length);
            this.collectionMock.Setup(x => x[It.IsAny<int>()]).Returns((int i) => requests[i]);
            this.patternParserMock.Setup(x => x.Parse(It.IsAny<StringBuilder>())).Returns(this.collectionMock.Object);
            this.libraryMock.Setup(x => x.GetFormatter(It.IsAny<FormatterRequest>())).Returns(formatterMock2.Object);
            this.formatterMock2.SetupGet(x => x.VariableMustExist).Returns(false);
            this.formatterMock2.Setup(x => x.Format(It.IsAny<string>(), It.IsAny<FormatterRequest>(),
                It.IsAny<IDictionary<string, object?>>(), null, It.IsAny<IMessageFormatter>())).Returns("formatted");
            
            Assert.Equal("formatted",subject.FormatMessage(Pattern, args));
            
            this.formatterMock2.VerifyAll();
        }

        #endregion
    }
}