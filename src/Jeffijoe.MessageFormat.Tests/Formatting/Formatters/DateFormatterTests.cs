using System;
using System.Globalization;
using Jeffijoe.MessageFormat.Formatting;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters;

public class DateFormatterTests
{
    [Theory]
    [InlineData("en-US", "1994-09-06T15:00:00Z", "9/6/1994")]
    [InlineData("da-DK", "1994-09-06T15:00:00Z", "06.09.1994")]
    public void DateFormatter_Short(string locale, string dateStr, string expected)
    {
        var mf = new MessageFormatter(locale: locale);
        var actual = mf.FormatMessage("{value, date}", new
        {
            value = DateTimeOffset.Parse(dateStr)
        });

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("en-US", "1994-09-06T15:00:00Z", "Tuesday, September 6, 1994")]
    [InlineData("da-DK", "1994-09-06T15:00:00Z", "tirsdag den 6. september 1994")]
    public void DateFormatter_Full(string locale, string dateStr, string expected)
    {
        var mf = new MessageFormatter(locale: locale);
        var actual = mf.FormatMessage("{value, date, full}", new
        {
            value = DateTimeOffset.Parse(dateStr)
        });

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DateFormatter_UnsupportedStyle()
    {
        var mf = new MessageFormatter();
        Assert.Throws<UnsupportedFormatStyleException>(
            () => mf.FormatMessage("{value, date, long}", new
            {
                value = DateTimeOffset.UtcNow
            }));
    }
        
    [Fact]
    public void DateFormatter_Custom()
    {
        var formatter = new CustomValueFormatters
        {
            Date = (CultureInfo culture, object? value, string? _, out string? formatted) =>
            {
                // This is just a test, you probably shouldn't be doing this in real workloads.
                formatted = ((FormattableString)$"{value:MMMM d 'in the year' yyyy}").ToString(culture);
                return true;
            }
        };
        var mf = new MessageFormatter(locale: "en-US", customValueFormatter: formatter);
        var actual = mf.FormatMessage("{value, date, long}", new
        {
            value = DateTimeOffset.Parse("1994-09-06T15:00:00Z")
        });

        Assert.Equal("September 6 in the year 1994", actual);
    }
}