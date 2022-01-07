// MessageFormat for .NET
// - PatternParser_Parse_Tests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Text;

using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests.Parsing
{
    /// <summary>
    /// The pattern parser_ parse_ tests.
    /// </summary>
    public class PatternParserParseTests
    {
        #region Fields

        /// <summary>
        /// The output helper.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternParserParseTests"/> class.
        /// </summary>
        /// <param name="outputHelper">
        /// The output helper.
        /// </param>
        public PatternParserParseTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expectedKey">
        /// The expected key.
        /// </param>
        /// <param name="expectedFormat">
        /// The expected format.
        /// </param>
        /// <param name="expectedArgs">
        /// The expected args.
        /// </param>
        [Theory]
        [InlineData("test, select, args", "test", "select", "args")]
        [InlineData("test, select, stuff {dawg}", "test", "select", "stuff {dawg}")]
        [InlineData("test, select, stuff {dawg's}", "test", "select", "stuff {dawg's}")]
        [InlineData("test, select, stuff {dawg''s}", "test", "select", "stuff {dawg''s}")]
        [InlineData("test, select, stuff '{{dawg}}'", "test", "select", "stuff '{{dawg}}'")]
        [InlineData("test, select, stuff {dawg, select, {name is '{'{name}'}'}}", "test", "select",
            "stuff {dawg, select, {name is '{'{name}'}'}}")]
        public void Parse(string source, string expectedKey, string expectedFormat, string expectedArgs)
        {
            var literalParserMock = new Mock<ILiteralParser>();
            var sb = new StringBuilder(source);
            literalParserMock.Setup(x => x.ParseLiterals(sb));
            literalParserMock.Setup(x => x.ParseLiterals(sb))
                             .Returns(new[] { new Literal(0, source.Length, 1, 1, source) });

            var subject = new PatternParser(literalParserMock.Object);

            // Warm up (JIT)
            Benchmark.Start("Parsing formatter patterns (first time before JIT)", this.outputHelper);
            subject.Parse(sb);
            Benchmark.End(this.outputHelper);
            Benchmark.Start("Parsing formatter patterns (after warm-up)", this.outputHelper);
            var actual = subject.Parse(sb);
            Benchmark.End(this.outputHelper);
            Assert.Single(actual);
            var first = actual[0];
            Assert.Equal(expectedKey, first.Variable);
            Assert.Equal(expectedFormat, first.FormatterName);
            Assert.Equal(expectedArgs, first.FormatterArguments);
        }

        /// <summary>
        /// The parse_exits_early_when_no_literals_have_been_found.
        /// </summary>
        [Fact]
        public void Parse_exits_early_when_no_literals_have_been_found()
        {
            var literalParserMock = new Mock<ILiteralParser>();
            var subject = new PatternParser(literalParserMock.Object);
            literalParserMock.Setup(x => x.ParseLiterals(It.IsAny<StringBuilder>())).Returns(new Literal[0]);
            Assert.Empty(subject.Parse(new StringBuilder()));
        }

        #endregion
    }
}