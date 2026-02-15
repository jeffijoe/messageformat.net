using System;
using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters;

/// <summary>
/// Represents the 'operations' for a given source number, as defined by <see href="https://unicode.org/reports/tr35/tr35-numbers.html#operands">Unicode TR35/LDML</see>.
/// </summary>
internal readonly struct PluralContext
{
    public PluralContext(int number)
    {
        Number = number;
        N = Math.Abs(number);
        I = number;
        V = 0;
        W = 0;
        F = 0;
        T = 0;
        C = 0;
        E = 0;
    }
        
    public PluralContext(decimal number) : this(number.ToString(CultureInfo.InvariantCulture), (double) number)
    {
    }

    public PluralContext(double number) : this(number.ToString(CultureInfo.InvariantCulture), number)
    {
    }

    /// <summary>
    /// Represents operands for a source number in string format.
    /// This library treats the input as a stringified double and does not currently parse out
    /// compact decimal forms (e.g., "1.25c4").
    /// </summary>
    public PluralContext(string number) : this(number, double.Parse(number, CultureInfo.InvariantCulture))
    {
    }

    /// <summary>
    /// Common constructor for parsing out operands from a stringified number.
    /// </summary>
    /// <remarks>
    /// The values of <see cref="V"/>, <see cref="W"/>, <see cref="F"/>, and <see cref="T"/> are all derived
    /// from the fractional part of the number, so it's important <paramref name="number"/> be parsable as a number.
    /// </remarks>
    /// <param name="number">The number in string form, as a decimal (not scientific/compact form).</param>
    /// <param name="parsed">The number pre-parsed as a double.</param>
    private PluralContext(string number, double parsed)
    {
        Number = parsed;
        N = Math.Abs(parsed);
        I = (int) parsed;

        var dotIndex = number.IndexOf('.');
        if (dotIndex == -1)
        {
            V = 0;
            W = 0;
            F = 0;
            T = 0;
            C = 0;
            E = 0;
        }
        else
        {
#if NET5_0_OR_GREATER
                var fractionSpan = number.AsSpan(dotIndex + 1, number.Length - dotIndex - 1);
                var fractionSpanWithoutZeroes = fractionSpan.TrimEnd('0');
#else
            var fractionSpan = number.Substring(dotIndex + 1, number.Length - dotIndex - 1);
            var fractionSpanWithoutZeroes = fractionSpan.TrimEnd('0');
#endif

            V = fractionSpan.Length;
            W = fractionSpanWithoutZeroes.Length;
            F = int.Parse(fractionSpan);
            T = int.Parse(fractionSpanWithoutZeroes);

            // The compact decimal exponent representations are not used in this library as operands are
            // always assumed to be parsable numbers.
            C = 0;
            E = 0;
        }
    }

    /// <summary>
    /// The 'source number' being evaluated for pluralization.
    /// </summary>
    public double Number { get; }

    /// <summary>
    /// The absolute value of <see cref="Number"/>.
    /// </summary>
    public double N { get; }

    /// <summary>
    /// The integer digits of <see cref="Number"/>.
    /// </summary>
    /// <example>
    /// 22.6 -> I = 22
    /// </example>
    public int I { get; }

    /// <summary>
    /// The count of visible fraction digits of <see cref="Number"/>, with trailing zeroes.
    /// </summary>
    /// <example>
    /// 1.450 -> V = 3
    /// </example>
    public int V { get; }

    /// <summary>
    /// The count of visible fraction digits of <see cref="Number"/>, without trailing zeroes.
    /// </summary>
    /// <example>
    /// 1.450 -> W = 2
    /// </example>
    public int W { get; }

    /// <summary>
    /// The visible fraction digits of <see cref="Number"/>, with trailing zeroes, as an integer.
    /// </summary>
    /// <example>
    /// 1.450 -> F = 450
    /// </example>
    public int F { get; }

    /// <summary>
    /// The visible fraction digits of <see cref="Number"/>, without trailing zeroes, as an integer.
    /// </summary>
    /// <example>
    /// 1.450 -> T = 45
    /// </example>
    public int T { get; }

    /// <summary>
    /// The compact decimal exponent of <see cref="Number"/>, in such cases where <see cref="Number"/>
    /// is represented as "[x]cC" such that <see cref="Number"/> == x * 10^C.
    /// </summary>
    /// <example>
    /// 1.25c4 -> C = 4
    /// 125c2 -> C = 2
    /// 12500 -> C = 0, as the number is not represented in compact decimal form.
    /// </example>
    public int C { get; }

    /// <summary>
    /// Deprecated (in LDML) synonym for <see cref="C"/>, reserved for future use by the standard.
    /// </summary>
    public int E { get; }
}