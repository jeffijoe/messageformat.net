// PatternParser_with_real_LiteralParser.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Linq;
using System.Text;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    public class PatternParser_with_real_LiteralParser
    {
        [Fact]
        public void Parse()
        {
            var subject = new PatternParser(new LiteralParser());

            const string source = @"Hi, {Name, select,
                                male={guy} female={gal}}, you have {count, plural, 
                                zero {no friends}, other {# friends}
                                }";
            Benchmark.Start("First run (warm-up)");
            subject.Parse(new StringBuilder(source));
            Benchmark.End();

            Benchmark.Start("Next one (warmed up)");
            var actual = subject.Parse(new StringBuilder(source));
            Benchmark.End();
            Assert.Equal(2, actual.Count());
            var formatterParam = actual.First();
            Assert.Equal("Name", formatterParam.Variable);
            Assert.Equal("select", formatterParam.FormatterName);
            Assert.Equal("male={guy} female={gal}", formatterParam.FormatterArguments);

            formatterParam = actual.ElementAt(1);
            Assert.Equal("count", formatterParam.Variable);
            Assert.Equal("plural", formatterParam.FormatterName);
            Assert.Equal("zero {no friends}, other {# friends}", formatterParam.FormatterArguments);
        }
    }
}