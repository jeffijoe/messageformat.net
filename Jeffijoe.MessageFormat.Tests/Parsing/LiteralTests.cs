// LiteralTests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Text;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    public class LiteralTests
    {
        [Fact]
        public void ShiftIndices()
        {
            var subject = new Literal(20, 29, 1, 1, new StringBuilder(new string('a', 10)));
            var other = new Literal(5, 10, 1, 1, new StringBuilder(new string('a', 6)));
            
            subject.ShiftIndices(2, other);
            // I honestly have no explanation for this, but it works with the formatter. Magic?
            Assert.Equal(18, subject.StartIndex);
            Assert.Equal(27, subject.EndIndex);
        } 
    }
}