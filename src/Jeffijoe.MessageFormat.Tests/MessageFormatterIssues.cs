// MessageFormat for .NET
// - MessageFormatter_full_integration_tests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    /// Issue cases.
    /// </summary>
    public class MessageFormatterIssues
    {
        [Fact]
        public void Issue13()
        {
            string plural = @"{num_guests, plural, offset:1, other {# {host} invites # people to their party.}}";
            string broken = @"{num_guests, plural, offset:1, other {{host} invites # people to their party.}}";

            var mf = new MessageFormatter();
            var vars = new { num_guests = "5", host = "Mary" };
            Assert.Equal("Mary invites 4 people to their party.", mf.FormatMessage(broken, vars));
            Assert.Equal("4 Mary invites 4 people to their party.", mf.FormatMessage(plural, vars));
        }
    }
}