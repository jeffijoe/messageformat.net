using System;
using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
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

        public PluralContext(string number) : this(number, double.Parse(number, CultureInfo.InvariantCulture))
        {
        }

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
#if NET5_0
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
                C = 0;
                E = 0;
            }
        }

        public double Number { get; }

        public double N { get; }

        public int I { get; }

        public int V { get; }

        public int W { get; }

        public int F { get; }

        public int T { get; }

        public int C { get; }

        public int E { get; }
    }
}