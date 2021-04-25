using System;
using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    public readonly struct PluralContext
    {
        public PluralContext(decimal number) : this(number.ToString(CultureInfo.InvariantCulture))
        {

        }

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

        public PluralContext(string number)
        {
            var dotIndex = number.IndexOf(".", StringComparison.InvariantCulture);

            if (dotIndex == -1)
            {
                var parsed = int.Parse(number);

                Number = parsed;
                N = Math.Abs(parsed);
                I = parsed;
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
                var parsed = double.Parse(number);

                Number = parsed;
                N = Math.Abs(parsed);
                I = (int)parsed;
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