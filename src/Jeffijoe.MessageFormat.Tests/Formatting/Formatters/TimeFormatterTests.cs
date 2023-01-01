using System;
using System.Globalization;
using Jeffijoe.MessageFormat.Formatting;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters
{
    public class TimeFormatterTests
    {
        [Theory]
        [InlineData("en-US", "1994-09-06T15:01:23Z", "3:01 PM")]
        [InlineData("da-DK", "1994-09-06T15:01:23Z", "15.01")]
        public void TimeFormatter_Short(string locale, string dateStr, string expected)
        {
            var mf = new MessageFormatter(locale: locale);
            var actual = mf.FormatMessage("{value, time, short}", new
            {
                value = DateTimeOffset.Parse(dateStr)
            });

            // Replacing all whitespace due to a difference in formatting on macOS vs Linux.
            expected = expected.Replace(" ", "");
            actual = actual.Replace(" ", "");
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("en-US", "1994-09-06T15:01:23Z", "3:01:23 PM")]
        [InlineData("da-DK", "1994-09-06T15:01:23Z", "15.01.23")]
        public void TimeFormatter_Default(string locale, string dateStr, string expected)
        {
            var mf = new MessageFormatter(locale: locale);
            var actual = mf.FormatMessage("{value, time}", new
            {
                value = DateTimeOffset.Parse(dateStr)
            });

            // Replacing all whitespace due to a difference in formatting on macOS vs Linux.
            expected = expected.Replace(" ", "");
            actual = actual.Replace(" ", "");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TimeFormatter_UnsupportedStyle()
        {
            var mf = new MessageFormatter();
            Assert.Throws<UnsupportedFormatStyleException>(
                () => mf.FormatMessage("{value, time, lol}", new
                {
                    value = DateTimeOffset.UtcNow
                }));
        }
        
        [Fact]
        public void TimeFormatter_Custom()
        {
            var formatter = new CustomValueFormatters
            {
                Time = (CultureInfo _, object? value, string? _, out string? formatted) =>
                {
                    formatted = $"{value:hmm} nice";
                    return true;
                }
            };
            var mf = new MessageFormatter(locale: "en-US", customValueFormatter: formatter);
            var actual = mf.FormatMessage("{value, time, long}", new
            {
                value = DateTimeOffset.Parse("1994-09-06T16:20:09Z")
            });

            Assert.Equal("420 nice", actual);
        }
    }
}