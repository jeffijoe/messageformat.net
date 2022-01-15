// MessageFormat for .NET
// - MessageFormatterTests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using System.Text;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    ///     The message formatter tests.
    /// </summary>
    public class MessageFormatterTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The format message.
        /// </summary>
        [Fact]
        public void FormatMessage()
        {
            const string Pattern = "{name} has {messages, plural, other {# messages}}.";
            const string Expected = "Jeff has 123 messages.";
            var args = new Dictionary<string, object?> { { "name", "Jeff" }, { "messages", 123} };
            
            var actual = MessageFormatter.Format(Pattern, args);
            
            Assert.Equal(Expected, actual);
        }

        /// <summary>
        /// The unescape literals.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [InlineData(@"Hello '{buddy}', how are you '{doing}'?", "Hello {buddy}, how are you {doing}?")]
        [InlineData(@"Hello ''{buddy}'', how are you '{doing}'?", @"Hello '{buddy}', how are you {doing}?")]
        [InlineData(@"{''}", @"{'}")]
        public void UnescapeLiterals(string source, string expected)
        {
            var actual = MessageFormatter.UnescapeLiterals(new StringBuilder(source));
            Assert.Equal(expected, actual);
        }

        /// <summary>
        ///     Verifies that format message throws when variables are missing and the formatter requires it to exist.
        /// </summary>
        [Fact]
        public void VerifyFormatMessageThrowsWhenVariablesAreMissingAndTheFormatterRequiresItToExist()
        {
            const string Pattern = "{name}";

            // Note the missing "name" variable.
            var args = new Dictionary<string, object?> { { "messages", 1 } };

            var subject = new MessageFormatter();
            
            var ex = Assert.Throws<VariableNotFoundException>(() => subject.FormatMessage(Pattern, args));
            Assert.Equal("name", ex.MissingVariable);
        }
        
        /// <summary>
        ///     Verifies that format message allows non-existent variables when formatter allows it.
        /// </summary>
        [Fact]
        public void VerifyFormatMessageAllowsNonExistentVariablesWhenFormatterAllowsIt()
        {
            const string Pattern = "{name, fake}";

            // Note the missing "name" variable.
            var args = new Dictionary<string, object?> ();

            var library = new FormatterLibrary();
            library.Add(new TestFormatter(variableMustExist: false, formatterName: "fake"));
            var subject = new MessageFormatter(new PatternParser(), library, useCache: false);

            var actual = subject.FormatMessage(Pattern, args);
            
            Assert.Equal("formatted", actual);
        }

        #endregion

        #region Fakes

        private class TestFormatter : IFormatter
        {
            private readonly string formatterName;

            public TestFormatter(bool variableMustExist, string formatterName)
            {
                this.VariableMustExist = variableMustExist;
                this.formatterName = formatterName;
            }
            
            public bool VariableMustExist { get; }

            public bool CanFormat(FormatterRequest request) => request.FormatterName == this.formatterName;

            public string Format(string locale, FormatterRequest request, IDictionary<string, object?> args, object? value,
                IMessageFormatter messageFormatter)
            {
                return "formatted";
            }
        }

        #endregion
    }
}