// LiteralTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Text;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    public class LiteralTests
    {
        [Fact]
        public void ShiftIndices()
        {
            var subject = new Literal(10, 20, 1, 1, new StringBuilder(new string('a', 10)));
            var other = new Literal(5, 9, 1, 1, new StringBuilder(new string('a', 4)));

            // The formatted length is 2, the pattern length was 4, 
            // that should shift it 2 (4 - 2 = 2) to the left,
            // which is why it has gone from 10 to 8, and 20 to 18.
            subject.ShiftIndices(2, other);
            Assert.Equal(8, subject.StartIndex);
            Assert.Equal(18, subject.EndIndex);
        } 
    }
}