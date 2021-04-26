using Jeffijoe.MessageFormat.Formatting.Formatters;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.MetadataGenerator
{
    public class PluralContextTests
    {
        [Theory]
        [InlineData("-12312.213213", 12312.213213)]
        [InlineData("12312.213213", 12312.213213)]
        [InlineData("-12312", 12312)]
        [InlineData("12312", 12312)]
        [InlineData("0", 0)]
        public void Parses_N(string s, double expectedN)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedN, ctx.N);
        }

        [Theory]
        [InlineData("-12312.213213", -12312)]
        [InlineData("12312.213213", 12312)]
        [InlineData("-12312", -12312)]
        [InlineData("12312", 12312)]
        [InlineData("0", 0)]
        public void Parses_I(string s, double expectedI)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedI, ctx.I);
        }

        [Theory]
        [InlineData("-12312.213213", 6)]
        [InlineData("12312.213213", 6)]
        [InlineData("12312.2132130", 7)]
        [InlineData("-12312", 0)]
        [InlineData("12312", 0)]
        [InlineData("0", 0)]
        public void Parses_V(string s, double expectedV)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedV, ctx.V);
        }

        [Theory]
        [InlineData("-12312.213213", 6)]
        [InlineData("12312.213213", 6)]
        [InlineData("12312.2132130", 6)]
        [InlineData("-12312", 0)]
        [InlineData("12312", 0)]
        [InlineData("0", 0)]
        public void Parses_W(string s, double expectedW)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedW, ctx.W);
        }

        [Theory]
        [InlineData("-12312.213213", 213213)]
        [InlineData("12312.213213", 213213)]
        [InlineData("12312.2132130", 2132130)]
        [InlineData("-12312", 0)]
        [InlineData("12312", 0)]
        [InlineData("0", 0)]
        public void Parses_F(string s, double expectedF)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedF, ctx.F);
        }

        [Theory]
        [InlineData("-12312.213213", 213213)]
        [InlineData("12312.213213", 213213)]
        [InlineData("12312.2132130", 213213)]
        [InlineData("-12312", 0)]
        [InlineData("12312", 0)]
        [InlineData("0", 0)]
        public void Parses_T(string s, double expectedT)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedT, ctx.T);
        }


        /// <summary>
        /// Exponents not supported yet
        /// </summary>
        [Theory]
        [InlineData("-12312.213213", 0)]
        [InlineData("12312.213213", 0)]
        [InlineData("12312.2132130", 0)]
        [InlineData("-12312", 0)]
        [InlineData("12312", 0)]
        [InlineData("0", 0)]
        public void Parses_C_And_E(string s, double expectedC)
        {
            var ctx = new PluralContext(s);

            Assert.Equal(expectedC, ctx.C);
            Assert.Equal(expectedC, ctx.E);
        }
    }
}
