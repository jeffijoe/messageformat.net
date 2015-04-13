// MessageFormat for .NET
// - PatternParser_with_real_LiteralParser.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Linq;
using System.Text;

using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    public class PatternParser_with_real_LiteralParser
    {
        private readonly ITestOutputHelper outputHelper;

        public PatternParser_with_real_LiteralParser(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void Parse()
        {
            var subject = new PatternParser(new LiteralParser());

            const string source = @"Hi, {Name, select,
                                male={guy} female={gal}}, you have {count, plural, 
                                zero {no friends}, other {# friends}
                                }";
            Benchmark.Start("First run (warm-up)", this.outputHelper);
            subject.Parse(new StringBuilder(source));
            Benchmark.End(this.outputHelper);

            Benchmark.Start("Next one (warmed up)", this.outputHelper);
            var actual = subject.Parse(new StringBuilder(source));
            Benchmark.End(this.outputHelper);
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