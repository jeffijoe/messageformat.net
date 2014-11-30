// FormatterLibraryTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Moq;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting
{
    public class FormatterLibraryTests
    {
        [Fact]
        public void GetFormatter()
        {
            var subject = new FormatterLibrary();
            var mock1 = new Mock<IFormatter>();
            var mock2 = new Mock<IFormatter>();

            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), "test", null, null);
            subject.Add(mock1.Object);
            subject.Add(mock2.Object);

            Assert.Throws<FormatterNotFoundException>(() => subject.GetFormatter(req));

            mock2.Setup(x => x.CanFormat(req)).Returns(true);
            var actual = subject.GetFormatter(req);
            Assert.Same(mock2.Object, actual);

            mock1.Setup(x => x.CanFormat(req)).Returns(true);
            actual = subject.GetFormatter(req);
            Assert.Same(mock1.Object, actual);
        }
    }
}