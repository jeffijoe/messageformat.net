// MessageFormat for .NET
// - SelectFormatterTests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;

using Moq;

using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters
{
    public class SelectFormatterTests
    {
        public static IEnumerable<object[]> Format_tests
        {
            get
            {
                yield return new object[] { "male {he said} female {she said} other {they said}", "male", "he said" };

                yield return new object[] { "male {he said} female {she said} other {they said}", "female", "she said" }
                    ;
                yield return new object[] { "male {he said} female {she said} other {they said}", "dawg", "they said" };
            }
        }

        [Theory]
        [PropertyData("Format_tests")]
        public void Format(string formatterArgs, string keyToUse, string expectedBlock)
        {
            var subject = new SelectFormatter();
            var messageFormatterMock = new Mock<IMessageFormatter>();
            messageFormatterMock.Setup(x => x.FormatMessage(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                                .Returns((string input, Dictionary<string, object> a) => input);
            var req = new FormatterRequest(
                new Literal(1, 1, 1, 1, new StringBuilder()), 
                "gender", 
                "select", 
                formatterArgs);
            var args = new Dictionary<string, object> { { "gender", keyToUse } };
            var result = subject.Format("en", req, args, messageFormatterMock.Object);
            Assert.Equal(expectedBlock, result);
        }
    }
}