using System.Globalization;
using Jeffijoe.MessageFormat.Formatting;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters;

public class NumberFormatterTests
{
    [Theory]
    [InlineData(69, "69")]
    [InlineData(69.420, "69.42")]
    [InlineData(123_456.789, "123,456.789")]
    [InlineData(1234567.1234567, "1,234,567.123")]
    public void NumberFormatter_Default(decimal number, string expected)
    {
        var mf = new MessageFormatter(locale: "en-US");
        // NOTE: The whitespace at the end is on purpose to cover whitespace tolerance in parsing.
        var actual = mf.FormatMessage("{value, number }", new
        {
            value = number
        });

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(69, "69.0000")]
    [InlineData(69.420, "69.4200")]
    public void NumberFormatter_Decimal_CustomFormat(decimal number, string expected)
    {
        var formatters = new CustomValueFormatters
        {
            Number = (CultureInfo culture, object? value, string? style, out string? formatted) =>
            {
                formatted = string.Format(culture, $"{{0:{style}}}", value);
                return true;
            }
        };
        var mf = new MessageFormatter(locale: "en-US", customValueFormatter: formatters);

        var actual = mf.FormatMessage("{value, number, 0.0000}", new
        {
            value = number
        });

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0.2, "20%")]
    [InlineData(1.2, "120%")]
    [InlineData(1234567.1234567, "123,456,712%")]
    public void NumberFormatter_Percent(decimal number, string expected)
    {
        var mf = new MessageFormatter(locale: "en-US");
            
        // NOTE: The inconsistent whitespace in the pattern is to cover whitespace tolerance in parsing.
        var actual = mf.FormatMessage("{value, number,percent}", new
        {
            value = number
        });

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0.2, "0")]
    [InlineData(1.2, "1")]
    [InlineData(2.7, "3")]
    [InlineData("2.7", "3")]
    [InlineData("a string", "a string")]
    [InlineData(true, "True")]
    public void NumberFormatter_Integer(object? value, string expected)
    {
        var mf = new MessageFormatter(locale: "en-US");
        var actual = mf.FormatMessage("{value, number, integer}", new
        {
            value
        });

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("en-US", 20, "$20.00")]
    [InlineData("en-US", 99.99, "$99.99")]
    [InlineData("da-DK", 99.99, "99,99 kr.")]
    public void NumberFormatter_Currency(string locale, decimal number, string expected)
    {
        var mf = new MessageFormatter(locale: locale);

        // NOTE: The inconsistent whitespace in the pattern is to cover whitespace tolerance in parsing.
        var actual = mf.FormatMessage("{value, number, currency }", new
        {
            value = number
        });

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NumberFormatter_ThrowsIfStyleIsNotSupported()
    {
        const decimal Number = 12.34m;
        var mf = new MessageFormatter(locale: "en-US");
        var ex = Assert.Throws<UnsupportedFormatStyleException>(() =>
            mf.FormatMessage($"{{value, number, wow}}",
                new
                {
                    value = Number
                }));
        Assert.Equal("value", ex.Variable);
        Assert.Equal("number", ex.Format);
        Assert.Equal("wow", ex.Style);
    }

    [Fact]
    public void NumberFormatter_BadInput_FallsBackToRegularFormat()
    {
        var mf = new MessageFormatter(locale: "en-US");

        {
            var actual = mf.FormatMessage($"{{value, number, currency}}", new
            {
                value = "a lot of money"
            });

            Assert.Equal("a lot of money", actual);
        }

        {
            var actual = mf.FormatMessage($"{{value, number, integer}}", new
            {
                value = "a lot of money"
            });

            Assert.Equal("a lot of money", actual);
        }

        {
            var actual = mf.FormatMessage($"{{value, number, integer}}", new
            {
                value = true
            });

            Assert.Equal("True", actual);
        }
    }
}