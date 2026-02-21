using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Jeffijoe.MessageFormat.Formatting;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters;

public partial class TimeFormatterTests
{
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en");

    [Theory]
    [InlineData("en-US", "1994-09-06T15:01:23Z", "3:01 PM")]
    [InlineData("da-DK", "1994-09-06T15:01:23Z", "15.01")]
    public void TimeFormatter_Short(string locale, string dateStr, string expected)
    {
        var mf = new MessageFormatter();
        var actual = mf.FormatMessage("{value, time, short}", new
        {
            value = DateTimeOffset.Parse(dateStr)
        }, CultureInfo.GetCultureInfo(locale));

        // Replacing all whitespace due to a difference in formatting on macOS vs Linux.
        expected = Normalize(expected);
        actual = Normalize(actual);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("en-US", "1994-09-06T15:01:23Z", "3:01:23 PM")]
    [InlineData("da-DK", "1994-09-06T15:01:23Z", "15.01.23")]
    public void TimeFormatter_Default(string locale, string dateStr, string expected)
    {
        var mf = new MessageFormatter();
        var actual = mf.FormatMessage("{value, time}", new
        {
            value = DateTimeOffset.Parse(dateStr)
        }, CultureInfo.GetCultureInfo(locale));

        // Replacing all whitespace due to a difference in formatting on macOS vs Linux.
        expected = Normalize(expected);
        actual = Normalize(actual);
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
            Time = (_, value, _, out formatted) =>
            {
                formatted = $"{value:hmm} nice";
                return true;
            }
        };
        var mf = new MessageFormatter(customValueFormatter: formatter);
        var actual = mf.FormatMessage("{value, time, long}", new
        {
            value = DateTimeOffset.Parse("1994-09-06T16:20:09Z")
        }, En);

        Assert.Equal("420 nice", actual);
    }

    [GeneratedRegex("\\s")]
    private static partial Regex WhitespaceRegex();

    private static string Normalize(string input)
    {
        return WhitespaceRegex().Replace(input, string.Empty);
    }
}
