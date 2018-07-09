// MessageFormat for .NET
// - BaseFormatterTests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests.Formatting
{
    /// <summary>
    ///     The base formatter tests.
    /// </summary>
    public class BaseFormatterTests
    {
        #region Fields

        /// <summary>
        ///     The output helper.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFormatterTests"/> class.
        /// </summary>
        /// <param name="outputHelper">
        /// The output helper.
        /// </param>
        public BaseFormatterTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the parse arguments_tests.
        /// </summary>
        public static IEnumerable<object[]> ParseArguments_tests
        {
            get
            {
                yield return
                    new object[]
                    {
                        "offset:1 test:1337 one {programmer}   other{programmers}",
                        new[] { "offset", "test" },
                        new[] { "1", "1337" },
                        new[] { "one", "other" },
                        new[] { "programmer", "programmers" }
                    };

                yield return
                    new object[]
                    {
                        "offset:1 test:1337 one\\123 {programmer}   other{programmers}",
                        new[] { "offset", "test" },
                        new[] { "1", "1337" },
                        new[] { "one\\123", "other" },
                        new[] { "programmer", "programmers" }
                    };
            }
        }

        /// <summary>
        ///     Gets the parse keyed blocks_tests.
        /// </summary>
        public static IEnumerable<object[]> ParseKeyedBlocks_tests
        {
            get
            {
                yield return
                    new object[]
                    {
                        "male {he} female {she}unknown{they}",
                        new[] { "male", "female", "unknown" },
                        new[] { "he", "she", "they" }
                    };
                yield return
                    new object[]
                    {
                        "zero {} other {wee}",
                        new[] { "zero", "other" },
                        new[] { string.Empty, "wee" }
                    };
                yield return new object[] { @"
                        male {he} 
                        female {she}
unknown
    {they}
", new[] { "male", "female", "unknown" }, new[] { "he", "she", "they" } };
                yield return new object[] { @"
                        male {he} 
                        female {she{dawg}}
unknown
    {they'{dawg}'}
", new[] { "male", "female", "unknown" }, new[] { "he", "she{dawg}", @"they'{dawg}'" } };
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The parse arguments.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="extensionKeys">
        /// The extension keys.
        /// </param>
        /// <param name="extensionValues">
        /// The extension values.
        /// </param>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <param name="blocks">
        /// The blocks.
        /// </param>
        [Theory]
        [MemberData("ParseArguments_tests")]
        public void ParseArguments(
            string args,
            string[] extensionKeys,
            string[] extensionValues,
            string[] keys,
            string[] blocks)
        {
            var subject = new BaseFormatterImpl();
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);
            var actual = subject.ParseArguments(req);

            Assert.Equal(extensionKeys.Length, actual.Extensions.Count());
            Assert.Equal(keys.Length, actual.KeyedBlocks.Count());

            for (int i = 0; i < actual.Extensions.ToArray().Length; i++)
            {
                var extension = actual.Extensions.ToArray()[i];
                Assert.Equal(extensionKeys[i], extension.Extension);
                Assert.Equal(extensionValues[i], extension.Value);
            }

            for (int i = 0; i < actual.KeyedBlocks.ToArray().Length; i++)
            {
                var block = actual.KeyedBlocks.ToArray()[i];
                Assert.Equal(keys[i], block.Key);
                Assert.Equal(blocks[i], block.BlockText);
            }
        }

        /// <summary>
        /// The parse arguments_invalid.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        [Theory]
        [InlineData("hello {{dawg}")]
        [InlineData("hello {dawg}}")]
        [InlineData("hello '{dawg}")]
        [InlineData("hello {dawg'}")]
        [InlineData("hello {dawg} {sweet}")]
        [InlineData("hello {dawg} test{sweet}}")]
        [InlineData("hello '{{dawg'}} test{sweet}")]
        public void ParseArguments_invalid(string args)
        {
            var subject = new BaseFormatterImpl();
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);
            var ex = Assert.Throws<MalformedLiteralException>(() => subject.ParseArguments(req));
            this.outputHelper.WriteLine(ex.Message);
        }

        /// <summary>
        /// The parse extensions.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="extension">
        /// The extension.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="expectedIndex">
        /// The expected index.
        /// </param>
        [Theory]
        [InlineData(" offset:3 boom", "offset", "3", 9)]
        [InlineData("testie:dawg lel", "testie", "dawg", 11)]
        public void ParseExtensions(string args, string extension, string value, int expectedIndex)
        {
            var subject = new BaseFormatterImpl();
            int index;
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);

            // Warmup
            subject.ParseExtensions(req, out index);

            Benchmark.Start("Parsing extensions a few times (warmed up)", this.outputHelper);
            for (int i = 0; i < 1000; i++)
            {
                subject.ParseExtensions(req, out index);
            }

            Benchmark.End(this.outputHelper);

            var actual = subject.ParseExtensions(req, out index);
            Assert.NotEmpty(actual);
            var first = actual.First();
            Assert.Equal(extension, first.Extension);
            Assert.Equal(value, first.Value);
            Assert.Equal(expectedIndex, index);
        }

        /// <summary>
        ///     The parse extensions_multiple.
        /// </summary>
        [Fact]
        public void ParseExtensions_multiple()
        {
            var subject = new BaseFormatterImpl();
            int index;
            var args = " offset:2 code:js ";
            var expectedIndex = 17;

            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);

            var actual = subject.ParseExtensions(req, out index);
            Assert.NotEmpty(actual);
            var result = actual.First();
            Assert.Equal("offset", result.Extension);
            Assert.Equal("2", result.Value);

            result = actual.ElementAt(1);
            Assert.Equal("code", result.Extension);
            Assert.Equal("js", result.Value);

            Assert.Equal(expectedIndex, index);
        }

        /// <summary>
        /// The parse keyed blocks.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        [Theory]
        [MemberData("ParseKeyedBlocks_tests")]
        public void ParseKeyedBlocks(string args, string[] keys, string[] values)
        {
            var subject = new BaseFormatterImpl();
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);

            // Warm-up
            subject.ParseKeyedBlocks(req, 0);

            Benchmark.Start("Parsing keyed blocks..", this.outputHelper);
            for (int i = 0; i < 10000; i++)
            {
                subject.ParseKeyedBlocks(req, 0);
            }

            Benchmark.End(this.outputHelper);

            var actual = subject.ParseKeyedBlocks(req, 0);
            Assert.Equal(keys.Length, actual.Count());
            this.outputHelper.WriteLine("Input: " + args);
            this.outputHelper.WriteLine("-----");
            for (int index = 0; index < actual.ToArray().Length; index++)
            {
                var keyedBlock = actual.ToArray()[index];
                var expectedKey = keys[index];
                var expectedValue = values[index];
                Assert.Equal(expectedKey, keyedBlock.Key);
                Assert.Equal(expectedValue, keyedBlock.BlockText);

                this.outputHelper.WriteLine("Key: " + keyedBlock.Key);
                this.outputHelper.WriteLine("Block: " + keyedBlock.BlockText);
            }
        }

        /// <summary>
        /// The parse keyed blocks unclosed_escape_sequence.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        [Theory]
        [InlineData("male {he} other {'{they}")]
        [InlineData("male {he} other {'# they}")]
        public void ParseKeyedBlocks_unclosed_escape_sequence(string args)
        {
            var subject = new BaseFormatterImpl();
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);

            Assert.Throws<MalformedLiteralException>(() => subject.ParseKeyedBlocks(req, 0));
        }

        #endregion

        /// <summary>
        ///     The base formatter impl.
        /// </summary>
        private class BaseFormatterImpl : BaseFormatter
        {
        }
    }
}