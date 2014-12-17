// MessageFormat for .NET
// - BaseFormatterTests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests.Formatting
{
    public class BaseFormatterTests
    {
        public static IEnumerable<object[]> ParseArguments_tests
        {
            get
            {
                yield return
                    new object[]
                    {
                        "offset:1 test:1337 one {programmer}   other{programmers}", new[] { "offset", "test" }, 
                        new[] { "1", "1337" }, new[] { "one", "other" }, new[] { "programmer", "programmers" }
                    };

                yield return
                    new object[]
                    {
                        "offset:1 test:1337 one\\123 {programmer}   other{programmers}", new[] { "offset", "test" }, 
                        new[] { "1", "1337" }, new[] { "one\\123", "other" }, new[] { "programmer", "programmers" }
                    };
            }
        }

        public static IEnumerable<object[]> ParseKeyedBlocks_tests
        {
            get
            {
                yield return
                    new object[]
                    {
                        "male {he} female {she}unknown{they}", new[] { "male", "female", "unknown" }, 
                        new[] { "he", "she", "they" }
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
    {they\{dawg\}}
", new[] { "male", "female", "unknown" }, new[] { "he", "she{dawg}", @"they\{dawg\}" } };
            }
        }

        #region Test classes

        private class BaseFormatterImpl : BaseFormatter
        {
        }

        #endregion

        [Theory]
        [InlineData("hello {{dawg}")]
        [InlineData("hello {dawg}}")]
        [InlineData("hello \\{dawg}")]
        [InlineData("hello {dawg\\}")]
        [InlineData("hello {dawg} {sweet}")]
        [InlineData("hello {dawg} test{sweet}}")]
        [InlineData("hello \\{{dawg\\}} test{sweet}")]
        public void ParseArguments_invalid(string args)
        {
            var subject = new BaseFormatterImpl();
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);
            var ex = Assert.Throws<MalformedLiteralException>(() => subject.ParseArguments(req));
            Console.WriteLine(ex.Message);
        }

        [Theory]
        [PropertyData("ParseArguments_tests")]
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

            Benchmark.Start("Parsing extensions a few times (warmed up)");
            for (int i = 0; i < 1000; i++)
            {
                subject.ParseExtensions(req, out index);
            }

            Benchmark.End();

            var actual = subject.ParseExtensions(req, out index);
            Assert.NotEmpty(actual);
            var first = actual.First();
            Assert.Equal(extension, first.Extension);
            Assert.Equal(value, first.Value);
            Assert.Equal(expectedIndex, index);
        }

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

        [Theory]
        [PropertyData("ParseKeyedBlocks_tests")]
        public void ParseKeyedBlocks(string args, string[] keys, string[] values)
        {
            var subject = new BaseFormatterImpl();
            var req = new FormatterRequest(new Literal(1, 1, 1, 1, new StringBuilder()), null, null, args);

            // Warm-up
            subject.ParseKeyedBlocks(req, 0);

            Benchmark.Start("Parsing keyed blocks..");
            for (int i = 0; i < 10000; i++)
            {
                subject.ParseKeyedBlocks(req, 0);
            }

            Benchmark.End();

            var actual = subject.ParseKeyedBlocks(req, 0);
            Assert.Equal(keys.Length, actual.Count());
            Console.WriteLine("Input: " + args);
            Console.WriteLine("-----");
            for (int index = 0; index < actual.ToArray().Length; index++)
            {
                var keyedBlock = actual.ToArray()[index];
                var expectedKey = keys[index];
                var expectedValue = values[index];
                Assert.Equal(expectedKey, keyedBlock.Key);
                Assert.Equal(expectedValue, keyedBlock.BlockText);

                Console.WriteLine("Key: " + keyedBlock.Key);
                Console.WriteLine("Block: " + keyedBlock.BlockText);
            }
        }

        [Fact]
        public void AssertVariableExists()
        {
            var subject = new BaseFormatterImpl();
            var args = new Dictionary<string, object> { { "dawg", "wee" } };
            Assert.Throws<VariableNotFoundException>(() => subject.AssertVariableExists(args, "Test"));
            Assert.DoesNotThrow(() => subject.AssertVariableExists(args, "dawg"));
        }
    }
}