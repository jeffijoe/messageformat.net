// FormatterRequestCollectionTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Text;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    public class FormatterRequestCollectionTests
    {
        [Fact]
        public void ShiftIndices()
        {
            var subject = new FormatterRequestCollection();
            subject.Add(new FormatterRequest(
                new Literal(0, 9, 1, 1, new StringBuilder(new string('a', 10))),
                "test",
                "test",
                "test"
            ));
            subject.Add(new FormatterRequest(
                new Literal(10, 19, 1, 1, new StringBuilder(new string('a', 10))),
                "test",
                "test",
                "test"
            ));
            subject.Add(new FormatterRequest(
                new Literal(20, 29, 1, 1, new StringBuilder(new string('a', 10))),
                "test",
                "test",
                "test"
            ));
            subject.ShiftIndices(1, 4);
            Assert.Equal(0, subject[0].SourceLiteral.StartIndex);
            Assert.Equal(10, subject[1].SourceLiteral.StartIndex);

            Assert.Equal(14, subject[2].SourceLiteral.StartIndex);
        }
    }
}