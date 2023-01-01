// MessageFormat for .NET
// - MessageFormatter_full_integration_tests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    /// Issue cases.
    /// </summary>
    public class MessageFormatterIssues
    {
        [Fact]
        public void Issue13_Bad_escaping_on_pound_symbol()
        {
            string plural = @"{num_guests, plural, offset:1, other {# {host} invites # people to their party.}}";
            string broken = @"{num_guests, plural, offset:1, other {{host} invites # people to their party.}}";

            var mf = new MessageFormatter();
            var vars = new { num_guests = "5", host = "Mary" };
            Assert.Equal("Mary invites 4 people to their party.", mf.FormatMessage(broken, vars));
            Assert.Equal("4 Mary invites 4 people to their party.", mf.FormatMessage(plural, vars));
        }
        
        [Fact]
        public void Issue27_WhiteSpace_in_identifiers_is_ignored()
        {
            var subject = new MessageFormatter(false);
            var result = subject.FormatMessage("{ count, plural , one {1 thing} other {# things} }", new
            {
                count = 2
            });

            Assert.Equal("2 things", result);
        }

        [Fact]
        public void Issue31_IDictionary_interface_support()
        {
            var subject = new MessageFormatter(locale: "en-US");

            IDictionary<string, object> idict = new Dictionary<string, object>
            {
                ["string"] = "value"
            };
            
            IDictionary<string, object?> idictNullable = new Dictionary<string, object?>
            {
                ["string"] = "value"
            };

            Assert.Equal("value", subject.FormatMessage("{string}", idict));
            Assert.Equal("value", subject.FormatMessage("{string}", idictNullable!));
        }
    }
}